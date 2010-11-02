
namespace NXmlConnector.Model
{
    /// <summary>
    /// Статус котировок по инструментам.
    /// </summary>
    public enum QuotationStatus
    {
        Unknown,

        /// <summary>
        /// Операции разрешены.
        /// </summary>
        A,

        /// <summary>
        /// Операции запрещены.
        /// </summary>
        S
    }
}