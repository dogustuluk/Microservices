using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

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
    }
}
