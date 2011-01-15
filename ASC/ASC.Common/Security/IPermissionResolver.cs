#region usings

using ASC.Common.Security.Authorizing;

#endregion

namespace ASC.Common.Security
{
    public interface IPermissionResolver
    {
        bool Check(ISubject subject, params IAction[] actions);

        bool Check(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider,
                   params IAction[] actions);

        void Demand(ISubject subject, params IAction[] actions);

        void Demand(ISubject subject, ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider,
                    params IAction[] actions);
    }
}