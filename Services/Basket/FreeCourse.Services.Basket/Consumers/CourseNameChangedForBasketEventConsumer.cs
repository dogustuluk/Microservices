using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Shared.Messages;
using FreeCourse.Shared.Services;
using MassTransit;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Consumers
{
    public class CourseNameChangedForBasketEventConsumer : IConsumer<CourseNameChangedForBasketEvent>
    {
        private readonly RedisService _redisService;
        
        public CourseNameChangedForBasketEventConsumer(RedisService redisService)
        {
            _redisService = redisService;
            
        }

        public async Task Consume(ConsumeContext<CourseNameChangedForBasketEvent> context)
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
                        x.CourseName = context.Message.UpdatedBasketCourseName;
                        x.CourseId = context.Message.CourseId;
                       
                    });
                    await _redisService.GetDb().StringSetAsync(key, JsonConvert.SerializeObject(basketDto));
                }
            }
        }
    }
}
