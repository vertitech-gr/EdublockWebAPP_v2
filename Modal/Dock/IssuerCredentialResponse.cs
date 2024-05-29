using System;
using Edu_Block_dev.Modal.Base;

namespace Edu_Block_dev.Modal.Dock
{
	public class IssuerCredentialResponse: BaseCommandResponse
    {
        public IssuerCredentialResponse(bool success = true, string message = null) : base(success, message) { }
    }
}

