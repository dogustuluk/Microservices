﻿using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using Mass = MassTransit;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeCourse.Shared.Messages;

namespace FreeCourse.Services.Catalog.Services
{
    public class CourseService:ICourseService
    {
        private readonly IMongoCollection<Course> _courseCollection;
        private readonly IMongoCollection<Category> _categoryCollection;
        private readonly IMapper _mapper;
        private readonly Mass.IPublishEndpoint _publishEndpoint;
        public CourseService(IMapper mapper, IDatabaseSettings databaseSettings, Mass.IPublishEndpoint publishEndpoint)
        {
            var client = new MongoClient(databaseSettings.ConnectionStrings);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _courseCollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            /*desc
             * mongodb gibi nosql veri tabanı yapılarında ilişkisel bir durum olmadığı için, ilgili tablonun ikincil anahtarı olarak kurguladığımız alanları elle doldurmak zorundayız bir nevi. buradaki örnekte ilk olarak kurs tablosuna gidilip oradaki dataları çekiyoruz daha sonra if kontrolünde eğer herhangi bir courses var ise içerisine girip ikincil anahtarın olduğu; burada Category tablosu; alanın parent tablosuyla ilgili tablomuzdaki ikincil anahtar alanını eşleştiriyoruz.
             */
            var courses = await _courseCollection.Find(course => true).ToListAsync(); //Category hariç bir şekilde gelir.

            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            var course = await _courseCollection.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();

            if (course == null)
            {
                return Response<CourseDto>.Fail("Course not found", 404);
            }
            course.Category = await _categoryCollection.Find(x => x.Id == course.CategoryId).FirstAsync();

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course), 200);
        }

        public async Task<Response<List<CourseDto>>> GetAllByUserIdAsync(string userId)
        {
            var courses = await _courseCollection.Find<Course>(x => x.UserId ==userId).ToListAsync();

            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category = await _categoryCollection.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }
            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }

        public async Task<Response<CourseDto>> CreateAsync(CourseCreateDto courseCreateDto)
        {
            var newCourse = _mapper.Map<Course>(courseCreateDto);//dönüşüm yapıyoruz çünkü mongodb Course'u biliyor, CourseCreateDto'yu bilmez.

            newCourse.CreatedTime = DateTime.Now;

            await _courseCollection.InsertOneAsync(newCourse);

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), 200);
        }

        public async Task<Response<NoContent>> UpdateAsync(CourseUpdateDto courseUpdateDto)
        {
            var updateCourse = _mapper.Map<Course>(courseUpdateDto);

            /*desc
             * find ile x.id == courseUpdateDto.Id'si ile bul ve updateCourse ile değiştir.
             */
            var result = await _courseCollection.FindOneAndReplaceAsync(x => x.Id == courseUpdateDto.Id, updateCourse);

            if(result == null)
            {
                return Response<NoContent>.Fail("Course not found", 404);
            }

            /*eventual consistency publish
             * burada fakePaymentController'daki gibi bir kuyruk ismi yazmıyoruz. Burda bir kuyruk ismi belirlememize gerek yok çünkü bu bir event. yani bir kuyruğa göndermiyoruz. bu bir exchange'e gidecek. bu exchange'e bir kuyruk oluşturarak subscribe olan mikroservislerimiz olacak. örnek vermek gerekirse Order mikroservisim exchange'e bir kuyrukla beraber subscribe olacak. Basket mikroservisi de yine aynı şekilde bir kuyruk oluşturup subscribe olacak. bu event'i dinleyen iki tane mikroservisimiz olacak çünkü her ikisi de içerisinde course name'i barındırıyor.
             */
            await _publishEndpoint.Publish<CourseNameChangedEvent>(new CourseNameChangedEvent { CourseId = updateCourse.Id, UpdatedName = courseUpdateDto.Name });
            await _publishEndpoint.Publish<CourseNameAndPriceChangedForBasketEvent>(new CourseNameAndPriceChangedForBasketEvent { CourseId = updateCourse.Id, UserId = updateCourse.UserId, UpdatedBasketCourseName = courseUpdateDto.Name, UpdatedCoursePrice = courseUpdateDto.Price });

            return Response<NoContent>.Success(204);//body'si yok.
        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result = await _courseCollection.DeleteOneAsync(x => x.Id == id);
            if(result.DeletedCount > 0)
            {
                return Response<NoContent>.Success(204);
            }
            else
            {
                return Response<NoContent>.Fail("Course not found", 404);
            }
        }
    }
}
