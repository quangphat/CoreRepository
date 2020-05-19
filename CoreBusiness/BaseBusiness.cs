using my8ViewObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreBusiness
{
    public abstract class BaseBusiness
    {
        public readonly CurrentProcess _process;

        public BaseBusiness(CurrentProcess process)
        {
            _process = process;
        }
        public void AddError(string errorMessage, params object[] traceKeys)
        {
            _process.AddError(errorMessage, traceKeys);
        }
        public bool CheckIsNotLogin()
        {
            if (_process.IsGuest) return false;
            if (string.IsNullOrWhiteSpace(_process.PersonId))
            {
                AddError(ErrorRsx.error_login_expected);
                return true;
            }
            return false;
        }
    }
}
