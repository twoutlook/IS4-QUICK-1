// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        private static async Task Main()
        {
            // discover endpoints from metadata
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // NOTE by Mark, 10/31
            Console.WriteLine("1. done, HttpClient GetDiscoveryDocumentAsync");

            // request token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "client",
                ClientSecret = "secret",

                Scope = "api1"
            });

            // NOTE by Mark, 10/31
            Console.WriteLine("2. done, ToeknEndpoint is "+ disco.TokenEndpoint);
            if (tokenResponse.IsError)

            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            // NOTE by Mark, 10/31
            Console.WriteLine("3. done, Access Token as follows" );
            
            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            // NOTE by Mark, 10/31
            Console.WriteLine("4. response of calling API, as follows");

            var response = await apiClient.GetAsync("https://localhost:6001/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

            //NOTE by Mark,10/31
            //https://stackoverflow.com/questions/16215938/c-sharp-prevent-cmd-from-closing-automatically-after-running
            Console.WriteLine("5. Enter to close this CMD.");

            Console.ReadLine();
        }
    }
}