using FreeCourse.Web.Handler;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using FreeCourse.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;

namespace FreeCourse.Web.Extensions
{
    //extension metotlar static olarak tanımlanır
    public static class ServiceExtension
    {
        public static void AddHttpClientServices(this IServiceCollection services, IConfiguration Configuration)
        {
            var serviceApiSettings = Configuration.GetSection("ServiceApiSettings").Get<ServiceApiSettings>();

            //httpclient
            services.AddHttpClient<IClientCredentialTokenService, ClientCredentialTokenService>();

            services.AddHttpClient<IIdentityService, IdentityService>();

            services.AddHttpClient<ICatalogService, CatalogService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.Catalog.Path}");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            //photostock
            services.AddHttpClient<IPhotoStockService, PhotoStockService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.PhotoStock.Path}");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();
            services.AddHttpClient<IUserService, UserService>(opt =>
            {
                //UserService içerisinde kullanmış olduğum httpClient'ın base uri'si alt satıda vermiş olduğum BaseUri olacaktır.
                opt.BaseAddress = new Uri(serviceApiSettings.IdentityBaseUri);
            })//handler delegate'i ekle
                .AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            //basket
            services.AddHttpClient<IBasketService, BasketService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.Basket.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            //discount
            services.AddHttpClient<IDiscountService, DiscountService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.Discount.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();
        }
    }
}
