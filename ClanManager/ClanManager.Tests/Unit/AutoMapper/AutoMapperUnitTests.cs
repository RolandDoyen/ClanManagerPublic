using AutoMapper;
using ClanManager.BLL.Profiles;
using ClanManager.WEB.Profiles;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ClanManager.Tests.Unit.Automapper
{
    public class AutoMapperUnitTests
    {
        [Fact]
        public void AutoMapperConfiguration_IsValid()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ClanProfileBLL>();
                cfg.AddProfile<ClanProfileWEB>();
                cfg.AddProfile<UserProfileBLL>();
                cfg.AddProfile<UserProfileWEB>();
            });

            var provider = services.BuildServiceProvider();
            var mapper = provider.GetRequiredService<IMapper>();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
