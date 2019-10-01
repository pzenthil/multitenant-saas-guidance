﻿using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Claims;

namespace Tailspin.Surveys.Web.Security
{
    /// <summary>
    /// A ClaimAction that selects all top level values from the json user data and adds them as Claims.
    /// This excludes duplicate sets of names and values.
    /// </summary>
    public class MapAllClaimsAction : ClaimAction
    {
        public MapAllClaimsAction() : base("All", ClaimValueTypes.String)
        {
        }

        public override void Run(JObject userData, ClaimsIdentity identity, string issuer)
        {
            if (userData == null)
            {
                return;
            }
            foreach (var pair in userData)
            {
                var claimValue = userData.TryGetValue(pair.Key, out var value) ? value.ToString() : null;

                // Avoid adding a claim if there's a duplicate name and value. This often happens in OIDC when claims are
                // retrieved both from the id_token and from the user-info endpoint.
                var duplicate = identity.FindFirst(c => string.Equals(c.Type, pair.Key, StringComparison.OrdinalIgnoreCase)
                        && string.Equals(c.Value, claimValue, StringComparison.Ordinal)) != null;

                if (!duplicate)
                {
                    identity.AddClaim(new Claim(pair.Key, claimValue, ClaimValueTypes.String, issuer));
                }
            }
        }
    }
}
