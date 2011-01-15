using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ASC.Common.Utils.Processes
{
    public enum CmdLineJoinType
    {
        Space,
        Equals,
        OnlyValues
    }

    public class CmdLineParam
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public CmdLineJoinType JoinType { get; set; }
        public object Value { get; set; }

        public override string ToString()
        {
            return ToString(false);
        }

        private string GetSeparator()
        {
            return JoinType == CmdLineJoinType.Space ? " " : JoinType == CmdLineJoinType.OnlyValues ? "" : "=";
        }

        public string ToString(bool useShortName)
        {
            if (Value == null)
            {
                return string.Empty;
            }
            var strValue = Value.ToString();
            var paramBuilder = new StringBuilder();
            paramBuilder.Append(useShortName ? ShortName : Name);
            if (JoinType != CmdLineJoinType.OnlyValues)
            {
                paramBuilder.Append(GetSeparator());
                if (strValue.IndexOf(' ') != -1)
                {
                    paramBuilder.AppendFormat("\"{0}\"", strValue);
                }
                else
                {
                    paramBuilder.AppendFormat(strValue);
                }
            }
            return paramBuilder.ToString();
        }

        public static explicit operator bool(CmdLineParam x)
        {
            if (x.Value is bool)
            {
                return (bool)x.Value;
            }
            return false;
        }

    }

    public class CmdLine
    {
        private readonly List<CmdLineParam> _parameters = new List<CmdLineParam>();

        public CmdLine AddRange(IDictionary<string, object> @params)
        {
            return AddRange(@params, CmdLineJoinType.Space);
        }

        public CmdLine AddRange(IDictionary<string, object> @params, CmdLineJoinType joinType)
        {
            _parameters
                .AddRange(
                    @params.Select(
                        x =>
                        new CmdLineParam { Name = x.Key, ShortName = x.Key, JoinType = joinType, Value = x.Value }));
            return this;
        }

        public CmdLine AddRange(IEnumerable<string> @params)
        {
            _parameters
                .AddRange(
                    @params.Select(
                        x =>
                        new CmdLineParam { Name = x, ShortName = x, JoinType = CmdLineJoinType.OnlyValues, Value = true }));
            return this;
        }


        public CmdLine Add(IEnumerable<CmdLineParam> @params)
        {
            _parameters.AddRange(@params);
            return this;
        }

        public CmdLine Add(CmdLineParam param)
        {
            _parameters.Add(param);
            return this;
        }

        public CmdLine Add(string paramName, object value)
        {
            return Add(paramName, CmdLineJoinType.Space, value);
        }

        public CmdLine Add(string paramName, CmdLineJoinType joinType, object value)
        {
            return Add(paramName, paramName, joinType, value);
        }

        public CmdLine Add(string paramName, string shortName, CmdLineJoinType joinType, object value)
        {
            return
                Add(new CmdLineParam { Name = paramName, ShortName = shortName, JoinType = joinType, Value = value });
        }

        public string GetExecArgs()
        {
            return string.Join(" ", _parameters.Select(x => x.ToString()).ToArray());
        }

        public bool HasParam(string paramName)
        {
            return _parameters.FirstOrDefault(x => x.Name.Equals(paramName) || x.ShortName.Equals(paramName)) != null;
        }

        public T GetValue<T>(string paramName)
        {
            return GetValue<T>(paramName, null);
        }

        public T GetValue<T>(string paramName, Func<T> defaultValue)
        {
            if (paramName == null) throw new ArgumentNullException("paramName");
            var cmdArg = GetCmdArg(paramName);
            if (cmdArg != null)
            {

                if (cmdArg.Value.GetType().Equals(typeof(T)))
                {
                    return (T)cmdArg.Value;
                }
                try
                {
                    var converter = TypeDescriptor.GetConverter(cmdArg.Value);
                    if (converter != null && converter.CanConvertTo(typeof(T)))
                    {
                        return (T)converter.ConvertTo(cmdArg.Value, typeof(T));
                    }
                    //Try another
                    converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null && converter.CanConvertFrom(cmdArg.Value.GetType()))
                    {
                        return (T)converter.ConvertFrom(cmdArg.Value);
                    }
                }
                catch
                { }
            }
            return defaultValue != null ? defaultValue() : default(T);
        }

        private CmdLineParam GetCmdArg(string paramName)
        {
            return _parameters.FirstOrDefault(x => (x.Name.Equals(paramName) || x.ShortName.Equals(paramName)) && x.Value != null);
        }

        public static CmdLine Parse(string[] args)
        {
            return Parse(args, CmdLineJoinType.Space);
        }

        public static CmdLine Parse(string[] args, CmdLineJoinType joinType)
        {
            return joinType == CmdLineJoinType.Equals
                       ? ParseEqualsCmdLine(args)
                       : joinType == CmdLineJoinType.OnlyValues ? ParseOnlyValues(args) : ParseSpacedCmdLine(args);
        }

        private static CmdLine ParseOnlyValues(IEnumerable<string> args)
        {

            return new CmdLine().AddRange(args);
        }

        private static CmdLine ParseSpacedCmdLine(IList<string> args)
        {
            var cmdLine = new CmdLine();
            for (var i = 0; i < args.Count; i++)
            {
                //Add first as param
                if (args[i].IndexOf('=')!=-1)
                {
                    ParseSpaced(args[i],cmdLine);                    
                }
                else
                {
                    var param = new CmdLineParam { Name = args[i], ShortName = args[i], JoinType = CmdLineJoinType.Space };
                    if (i + 1 < args.Count && !string.IsNullOrEmpty(args[i + 1]))
                    {
                        param.Value = args[i + 1].Trim('"');
                        i++;
                    }
                    else
                    {
                        param.Value = true;
                        param.JoinType = CmdLineJoinType.OnlyValues;
                    }
                    cmdLine.Add(param);
                }
            }
            return cmdLine;
        }

        private static CmdLine ParseEqualsCmdLine(IEnumerable<string> args)
        {
            var cmdLine = new CmdLine();
            foreach (var paramEq in args)
            {
                ParseSpaced(paramEq, cmdLine);
            }
            return cmdLine;
        }

        private static void ParseSpaced(string paramEq, CmdLine cmdLine)
        {
            if (!string.IsNullOrEmpty(paramEq))
            {
                var values = paramEq.Split('=');
                var param = new CmdLineParam { Name = values[0], ShortName = values[0], JoinType = CmdLineJoinType.Equals };
                if (values.Length == 2 && !string.IsNullOrEmpty(values[1]))
                {
                    param.Value = values[1].Trim('"');
                }
                else
                {
                    param.Value = true;
                    param.JoinType = CmdLineJoinType.OnlyValues;
                }
                cmdLine.Add(param);
            }
        }

        public object this[string name]
        {
            get { return GetCmdArg(name).Value; }
            set
            {
                var arg = GetCmdArg(name);
                if (arg != null)
                {
                    arg.Value = value;
                }
                else
                {
                    Add(name, value);
                }
            }
        }


        public object this[int pos]
        {
            get { return _parameters.Count > pos ? _parameters[pos].Value : null; }
            set
            {
                if (_parameters.Count > pos)
                {
                    _parameters[pos].Value = value;
                }
            }
        }

        public override string ToString()
        {
            return GetExecArgs();
        }
    }
}