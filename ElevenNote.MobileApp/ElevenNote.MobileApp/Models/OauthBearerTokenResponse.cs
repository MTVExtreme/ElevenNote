﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ElevenNote.MobileApp.Models
{
    class OauthBearerTokenResponse
    {
        public string access_token { get; set; }

        public string token_type { get; set; }

        public string expires_in { get; set; }

        public string userName { get; set; }
    }
}
