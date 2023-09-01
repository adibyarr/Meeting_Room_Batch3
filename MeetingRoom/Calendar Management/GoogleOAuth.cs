using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;

namespace CalendarAPI;

public static class GoogleOAuth
{
    public static UserCredential GenerateCredential()
    {
        UserCredential credential;

        string[] scopes = { CalendarService.Scope.Calendar };

        using (var stream = new FileStream(".\\Calendar Management\\Credentials\\calendar-api-oauth.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                scopes, "bimanjaya", CancellationToken.None, new FileDataStore(".\\Calendar Management\\Credentials", true)).Result;
        }
        return credential;
    }
}
