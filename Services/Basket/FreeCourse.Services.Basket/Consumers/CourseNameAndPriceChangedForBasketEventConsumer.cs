using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Shared.Messages;
using FreeCourse.Shared.Services;
using MassTransit;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Consumers
{
    public class CourseNameAndPriceChangedForBasketEventConsumer : IConsumer<CourseNameAndPriceChangedForBasketEvent>
    {
        private readonly RedisService _redisService;
        
        public CourseNameAndPriceChangedForBasketEventConsumer(RedisService redisService)
        {
            _redisService = redisService;
            
        }

        public async Task Consume(ConsumeContext<CourseNameAndPriceChangedForBasketEvent> context)
        {
            var keys = _redisService.GetKeys();
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    var basket = await _redisService.GetDb().StringGetAsync(key);
                    //var basketDto = JsonSerializer.Deserialize<BasketDto>(basket);
                    var basketDto = JsonConvert.DeserializeObject<BasketDto>(basket);
                    basketDto.basketItems.ForEach(x =>
                    {
                        x.CourseId = context.Message.CourseId;
                        x.CourseName = context.Message.UpdatedBasketCourseName;
                        x.Price = context.Message.UpdatedCoursePrice;
                    });
                    await _redisService.GetDb().StringSetAsync(key, JsonConvert.SerializeObject(basketDto));
                }
            }
        }
    }
}
