using AvaloniaTest.Models.AddressSearch;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq.Protected;
using AvaloniaTest.Services;
using AvaloniaTest.Services.Interfaces;
using System.Diagnostics;

namespace Tests.UnitsTest
{
    public class AddressSearchControllerTest
    {


        [Fact]
        public async Task Search_ReturnsResults_WhenApiResponseIsValid()
        {
            // Arrange
            
            var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            //var dto = new Address { name = "Warsaw Praga", city = "Warsaw", country = "Poland" };
            var address = new Address
            {
                name = "Warsaw Praga",
                city = "Warsaw",
                country = "Poland"
            };

            var root = new Root
            {
                place_id = "12345",
                osm_id = "osm123",
                osm_type = "way",
                licence = "OpenStreetMap",
                lat = "52.2298",
                lon = "21.0118",
                boundingbox = new List<string> { "52.229", "52.230", "21.011", "21.012" },
                @class = "place",
                type = "city",
                display_name = "Warsaw Praga, Warsaw, Poland",
                display_place = "Warsaw",
                display_address = "Warsaw Praga, Poland",
                address = address
            };
            var mockResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new List<Root> { root })
            };

            mockHandler
                 .Protected()
                 .Setup<Task<HttpResponseMessage>>(
                 "SendAsync",
                 ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Get),
                 ItExpr.IsAny<CancellationToken>())
                 .ReturnsAsync(mockResponse);

            var httpClient = new HttpClient(mockHandler.Object);

            // Mockowanie odpowiedzi GetStringAsync
    
            var controller = new AddressSearchController(httpClient);

            // Act
            var result = await controller.Search("Warsaw");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Warsaw Praga", result[0].address.name);
        }

        [Fact]
        public async Task Search_ReturnsNull_WhenApiResponseIsEmpty()
        {
            // Arrange
            var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            // Tworzymy pustą odpowiedź HTTP
            var mockResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("") // Pusta zawartość
            };

            mockHandler
                 .Protected()
                 .Setup<Task<HttpResponseMessage>>(
                 "SendAsync",
                 ItExpr.Is<HttpRequestMessage>(m => m.Method == HttpMethod.Get),
                 ItExpr.IsAny<CancellationToken>())
                 .ReturnsAsync(mockResponse);

            var httpClient = new HttpClient(mockHandler.Object);
            var controller = new AddressSearchController(httpClient);

            // Act
            var result = await controller.Search("xyz");

            // Assert
            Assert.Null(result);
        }
 
    }


}
