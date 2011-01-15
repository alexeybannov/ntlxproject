using System;
using System.Collections;
using System.Text;
using ASC.Messenger.Client;
using ASC.Messenger.Common;
using ASC.Notify.Messages;
using ASC.Notify.Sinks;

namespace ASC.Core.Configuration.Service.Notify.Messenger {

	/// <summary>
	/// Предоставляет возможность отправки уведомлений при помощи сервиса сообщений.
	/// </summary>
	public class MessengerSenderSink : SinkSkeleton, ISenderSink {

		/// <summary>
		/// (get/set) Интервал, в течение которого не будет происходить попытки подключится
		/// к серверу, если нет соединения, чтоб поменьше тормозило. По умолчанию 1 минута.
		/// </summary>
		private TimeSpan TimeoutOnExceptionInterval {
			get;
			set;
		}

		/// <summary>
		/// (get/set) Флаг, указывающий надо ли выжидать определенный промежуток времени,
		/// между неудачними попытками подключения к серверу. По умолчанию true.
		/// </summary>
		private bool TimeOutOnException {
			get;
			set;
		}

		private MessengerServiceClient MessengerClient {
			get {
				if (messengerClient == null) messengerClient = new MessengerServiceClient();
				return messengerClient;
			}
		}

		private MessengerServiceClient messengerClient;

		private const string timeoutOnExceptionIntervalKey = "timeoutOnExceptionInterval";

		private const string timeoutOnExceptionKey = "timeoutOnException";

		private DateTime lastTryConnectDateTime;

		/// <summary>
		/// Создает эклемпляр класса MessengerSenderSink.
		/// </summary>
		public MessengerSenderSink()
			: base() {
			TimeoutOnExceptionInterval = TimeSpan.FromMinutes(1);
			TimeOutOnException = true;
		}

		/// <summary>
		/// Создает эклемпляр класса MessengerSenderSink.
		/// </summary>
		/// <remarks>
		/// В передаваемых свойствах можно указать настройки свойств TimeOutOnException и
		/// TimeoutOnExceptionInterval, указав ключи timeoutOnException и 
		/// timeoutOnExceptionInterval соответственно.
		/// </remarks>
		/// <param name="properties">Свойства объекта</param>
		public MessengerSenderSink(IDictionary properties)
			: base(properties) {

			TimeoutOnExceptionInterval = TimeSpan.FromMinutes(1);
			TimeOutOnException = true;
			if (properties.Contains(timeoutOnExceptionKey)) {
				TimeOutOnException = Convert.ToBoolean(properties[timeoutOnExceptionKey]);
			}
			if (properties.Contains(timeoutOnExceptionIntervalKey)) {
				TimeoutOnExceptionInterval = (TimeSpan)properties[timeoutOnExceptionIntervalKey];
			}
		}

		/// <inheritdoc/>
		public override SendResponse ProcessMessage(INoticeMessage message) {
			if (message == null) throw new ArgumentNullException("message");

			var response = new SendResponse(message,ASC.Core.Configuration.Constants.NotifyMessengerSenderSysName,SendResult.Ok);

			try {
				TryConnectMessengerClient();

				Guid toGuid = Guid.Empty;
				try {
					toGuid = new Guid(message.Recipient.ID);
				}
				catch {
					throw new ItemNotFoundException(message.Recipient.ID);
				}
				ISendable to = null;
				try {
					to = MessengerClient.GetItemById<ISendable>(toGuid);
				}
				catch (ConnectionNotFoundException) {
					//если были не подключены -- пробуем подключиться и снова берем получателя
					TryConnectMessengerClient();
					to = MessengerClient.GetItemById<ISendable>(toGuid);
				}

				string msgText = BuildMessage(message.Subject, message.Body);
				MessengerClient.MessageSend(MessageType.Text, to, msgText);
			}
			catch (ItemNotFoundException) {
				response.Result = SendResult.IncorrectRecipient;
			}
			catch (Exception ex) {
				response.Exception = ex;
				response.Result = SendResult.Impossible;
			}
			return response;
		}

		private void TryConnectMessengerClient() {
			if (TimeOutOnException && lastTryConnectDateTime != DateTime.MinValue) {
				//уже была попытка подключения
				if (messengerClient == null || !messengerClient.IsConnected) {
					//и, судя по всему, неудачная, поэтому выкинем ошибку, чтобы не тормозить
					if (lastTryConnectDateTime + TimeoutOnExceptionInterval < DateTime.Now) {
						throw new MessengerNotConnectedException();
					}
				}
			}
			lastTryConnectDateTime = DateTime.Now;

			if (!SecurityContext.CurrentAccount.Equals(Constants.CoreSystem)) {
				SecurityContext.AuthenticatMe(Constants.CoreSystem);
			}
			if (!MessengerClient.IsConnected) {
				MessengerClient.Connect();
			}
		}

		private string BuildMessage(string subject, string body) {
			//rtf документ: {\rtf1....................}
			const string RTF_HEADER = @"{\rtf1";

			bool isSubjectRtf = subject.StartsWith(RTF_HEADER);
			bool isBodyRtf = body.StartsWith(RTF_HEADER);

			if (!isSubjectRtf && !isBodyRtf) {
				//если у нас не rtf документы, то просто вернем строку
				return string.Format("{0}{1}{2}", subject, Environment.NewLine, body);
			}

			var sb = new StringBuilder();
			if (isSubjectRtf) {
				//выкинем закрывающую скобку
				sb.Append(subject.Substring(0, subject.Length - 1));
			}
			else {
				//создадим rtf
				sb.AppendFormat(@"{0}{1}", RTF_HEADER, subject);
			}
			sb.Append("\\par");//перенесем строку
			if (isBodyRtf) {
				//вычленим тело rtf документа, вышвырнув заголовок и последнюю закрывающую скобку
				sb.Append(body.Substring(RTF_HEADER.Length, body.Length - RTF_HEADER.Length - 1));
			}
			else {
				//тупо добавим текст
				sb.Append(body);
			}
			sb.Append("}");//закроем rtf документ

			//запишем все символы с кодом более 255 в utf формате
			var rtf = new StringBuilder();
			var rtfText = sb.ToString();
			for (int i = 0; i < rtfText.Length; i++) {
				if (255 < rtfText[i]) rtf.AppendFormat(@"\u{0}?", Convert.ToInt32(rtfText[i]));
				else rtf.Append(rtfText[i]);
			}

			return rtf.ToString();
		}
	}
}