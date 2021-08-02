using System;
using System.Collections.Generic;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Net;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace AwsDotnetCsharp
{
    public class Handler
    {
        private const string NOMBRE_TABLA = "Almacen-dev-AlmacenTable-20PIIR3XFKM1";
       public APIGatewayProxyResponse GetItem(APIGatewayProxyRequest request, ILambdaContext contex)
       {
           try{


            AmazonDynamoDBClient client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest3);
            var articuloAlmacen = JsonConvert.DeserializeObject<ArticuloAlmacen>(request.Body);
            if (request.PathParameters.Count == 0)
            {
                return GetItems(request, contex);
            }
            
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
            {
            { "Articulo", new AttributeValue { S = "Zapatillas"} }
            };

            // Create GetItem request
            GetItemRequest itemRequest = new GetItemRequest
            {
            TableName = NOMBRE_TABLA,
            Key = key,
            };

            var result =  client.GetItemAsync(itemRequest).Result;

            Dictionary<string, AttributeValue> item = result.Item;
            var cantidad = "";
            foreach (var keyValuePair in item)
            {
                cantidad = item["Cantidad"].ToString();
            }

            var response = CrearRespuestaHttp(cantidad, HttpStatusCode.OK);
            return response;
            }
            catch (Exception ex)
            {
                          
            var response = CrearRespuestaHttp(ex.Message, HttpStatusCode.InternalServerError);
            return response; 
            }
       }

       public APIGatewayProxyResponse GetItems(APIGatewayProxyRequest request, ILambdaContext contex)
       {
           try{


            AmazonDynamoDBClient client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest3);
            var articuloAlmacen = JsonConvert.DeserializeObject<ArticuloAlmacen>(request.Body);
            
            Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
            {
            { "Articulo", new AttributeValue { S = "Zapatillas"} }
            };

            // Create GetItem request
            GetItemRequest itemRequest = new GetItemRequest
            {
            TableName = NOMBRE_TABLA,
            Key = key,
            };

            var result =  client.GetItemAsync(itemRequest).Result;

            Dictionary<string, AttributeValue> item = result.Item;
            var cantidad = "";
            foreach (var keyValuePair in item)
            {
                cantidad = item["Cantidad"].ToString();
            }

            var response = CrearRespuestaHttp(cantidad, HttpStatusCode.OK);
            return response;
            }
            catch (Exception ex)
            {
                          
            var response = CrearRespuestaHttp(ex.Message, HttpStatusCode.InternalServerError);
            return response; 
            }
       }

       public APIGatewayProxyResponse PostItem(APIGatewayProxyRequest request, ILambdaContext contex)
       {
           var statusCode = HttpStatusCode.OK;
           var body = "body sin definir";

           try
           {
                AmazonDynamoDBClient client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest3);
                var articuloAlmacen = JsonConvert.DeserializeObject<ArticuloAlmacen>(request.Body);

                Dictionary<string, AttributeValue> key = new Dictionary<string, AttributeValue>
                {
                    { "Articulo", new AttributeValue { S = articuloAlmacen.Articulo} }
                };

                Dictionary<string, AttributeValueUpdate> updates = new Dictionary<string, AttributeValueUpdate>();
                updates["Cantidad"] = new AttributeValueUpdate()
                {
                    Action = AttributeAction.ADD,
                    Value = new AttributeValue { N = "100" }
                };

                UpdateItemRequest item = new UpdateItemRequest
                {
                    TableName = NOMBRE_TABLA,
                    Key = key,
                    AttributeUpdates = updates
                };

                var result = client.UpdateItemAsync(item).Result;
           }catch(Exception ex)
           {
                statusCode = HttpStatusCode.InternalServerError;
                body = ex.Message;
           }

            var response = CrearRespuestaHttp(body, statusCode);
            return response;
        }

       public APIGatewayProxyResponse PutItem(APIGatewayProxyRequest request, ILambdaContext contex)
       {
           var statusCode = HttpStatusCode.OK;
           var body = "body sin definir";

           try{
            AmazonDynamoDBClient client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.EUWest3);
            var articuloAlmacen = JsonConvert.DeserializeObject<ArticuloAlmacen>(request.Body);

            Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>();

            attributes["Articulo"] = new AttributeValue { S = articuloAlmacen.Articulo };
            attributes["Version"] = new AttributeValue { N = "1" };
            attributes["Cantidad"] = new AttributeValue { N = articuloAlmacen.Cantidad.ToString() };

            PutItemRequest item = new PutItemRequest
            {
                TableName = NOMBRE_TABLA,
                Item = attributes
            };

            PutItemResponse result = client.PutItemAsync(item).Result;

            body = request.Body;
           }catch(Exception ex)
           {
            statusCode = HttpStatusCode.InternalServerError;
            body = ex.Message;
           }

            var response = CrearRespuestaHttp(body, statusCode);
            return response;
       }

       public APIGatewayProxyResponse DeleteItem(APIGatewayProxyRequest request, ILambdaContext contex)
       {
            var response = CrearRespuestaHttp("DELETE ITEMS", HttpStatusCode.OK);
           return response;
       }

       public APIGatewayProxyResponse CrearRespuestaHttp(string body, HttpStatusCode statusCode)
       {
            var response = new APIGatewayProxyResponse()
            {
                StatusCode = (int)statusCode,
                Body = body
            };
           return response;
       }

        public class ArticuloAlmacen
        {   
            [JsonProperty("Articulo")]
            public string Articulo { get; set; }
            [JsonProperty("Cantidad")]
            public int Cantidad { get; set; }
        }
    }
}
