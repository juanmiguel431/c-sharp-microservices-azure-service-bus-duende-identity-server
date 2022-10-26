// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Mango.Services.Identity.Controllers.Consent;

namespace Mango.Services.Identity.Controllers.Device;

public class DeviceAuthorizationInputModel : ConsentInputModel
{
    public string UserCode { get; set; }
}