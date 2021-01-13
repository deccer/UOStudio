using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace UOStudio.Web.Controllers
{
    public class BaseController : ControllerBase
    {
        protected Guid GetUserId()
        {
            var nameId = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);
            if (nameId == null)
            {
                // TODO: handle
            }

            var userId = Guid.Parse(nameId!.Value);
            return userId;
        }
    }
}
