// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace FreeCourse.IdentityServer
{
    public static class Config
    {
        //audience
        public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
        {
            new ApiResource("resource_catalog"){Scopes ={ "catalog_fullpermission" }},
            new ApiResource("resource_photo_stock"){Scopes ={ "photo_stock_fullpermission" }},
            new ApiResource("resource_basket"){Scopes ={ "basket_fullpermission" }},
            new ApiResource(IdentityServerConstants.LocalApi.ScopeName)
        };

        //IdentityResources -> kullanıcı ile ilgili işlemler
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                       new IdentityResources.Email(), //email claim'i
                       new IdentityResources.OpenId(),//mutlaka olmak zorunda //sub claim'i
                       new IdentityResources.Profile(),
                       new IdentityResource()
                       {
                           Name = "roles",
                           DisplayName = "Roles",
                           Description = "Kullanıcı rolleri",
                           UserClaims = new []
                            {
                               "role" //role claim'i. Kendimiz tanımladık buradaki claim'i.
                            }
                       }
                   };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("catalog_fullpermission","Catalog API için full erişim"),
                new ApiScope("photo_stock_fullpermission","Photo Stock API için full erişim"),
                new ApiScope("basket_fullpermission","Basket API için full erişim"),
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName)
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    /*clientName
                     * best practise
                     * Belirtmemizin sebebi; eğer merkezi bir üyelik sistemi kullansaydık -> şu client senden şu şu bilgileri istiyor gibi bir izin isteyecekti.
                     */
                    ClientName = "Asp.Net Core MVC",
                    ClientId = "WebMvcClient",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "catalog_fullpermission", "photo_stock_fullpermission", IdentityServerConstants.LocalApi.ScopeName }
                },

                new Client
                {
                    ClientName = "Asp.Net Core MVC",
                    ClientId = "WebMvcClientForUser",
                    AllowOfflineAccess = true, //offline access için bu özelliği açmamız gerekmektedir.
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    /*ResourceOwnerPasswordAndClientCredentials
                     * Burada bunu tanımlayamayız çünkü client credentials'da refresh token yoktur. Bizim senaryomuzda refresh token'a ihtiyacımız vardır.
                     */
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    /*AllowedScopes
                     * OpenId'yi mutlaka almalıyız.
                     * OfflineAccess --> kullanıcı ofline olsa dahi kullanıcı adına elimizdeki refresh token ile birlikte yeni bir token alabiliriz; bu yüzden adı OfflineAccess.
                     */
                    AllowedScopes = 
                    {
                        "basket_fullpermission",
                        IdentityServerConstants.StandardScopes.Email, 
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile, 
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.LocalApi.ScopeName,
                        "roles" 
                    },
                    AccessTokenLifetime = 1*60*60, //1 saat
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = (int)(DateTime.Now.AddDays(60) - DateTime.Now).TotalSeconds, //60 gün
                    RefreshTokenUsage = TokenUsage.ReUse //refresh token tekrar kullanılabilir.
                }
            };
    }
}