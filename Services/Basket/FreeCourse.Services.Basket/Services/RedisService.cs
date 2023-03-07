using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourse.Services.Basket.Services
{
    public class RedisService
    {
        //db ile haberleşeceği için port ve host'a ihtiyacımız vardır.
        private readonly string _host;
        private readonly int _port;

        //redis bağlantısı için
        private ConnectionMultiplexer _connectionMultiplexer;
        public RedisService(string host, int port)
        {
            _host = host;
            _port = port;
        }

        //bağlantıyı verir.
        public void Connect() => _connectionMultiplexer = ConnectionMultiplexer.Connect($"{_host}:{_port}");

        //db'yi verir.
        public IDatabase GetDb(int db = 1) => _connectionMultiplexer.GetDatabase(db);

        //masstransit için. rediste sadece basket nesnelerini tuttuğumuz için şuanlık sıkıntı yok.
        public List<RedisKey> GetKeys() => _connectionMultiplexer.GetServer($"{_host}:{_port}").Keys(1).ToList();
    }
}
