using System;
using System.Diagnostics;

namespace ASC.Core.Users
{
    [DebuggerDisplay("DepartmentId = {DepartmentId}, ManagerId = {ManagerId}")]
    [Serializable]
    public class DepartmentManagerRef
    {
        public string Id
        {
            get { return string.Format("{0}{1}", DepartmentId, ManagerId); }
        }

        public Guid DepartmentId { get; private set; }

        public Guid ManagerId { get; private set; }

        public DepartmentManagerRef(Guid departmentId, Guid managerId)
        {
            DepartmentId = departmentId;
            ManagerId = managerId;
        }

        public override int GetHashCode()
        {
            return (DepartmentId.GetHashCode() ^ ManagerId.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            var r = obj as DepartmentManagerRef;
            return
                r != null &&
                r.DepartmentId == DepartmentId &&
                r.ManagerId == ManagerId;
        }
    }
}