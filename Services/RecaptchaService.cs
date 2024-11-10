// using System;
// using System.Collections.Generic;
// using System.Net.Http;
// using System.Text.Json;
// using System.Threading.Tasks;
// using Microsoft.Extensions.Configuration;

// public class RecaptchaService : IRecaptchaService
// {
//     private readonly IConfiguration _configuration;
//     private readonly HttpClient _httpClient;

//     public RecaptchaService(IConfiguration configuration, HttpClient httpClient)
//     {
//         _configuration = configuration;
//         _httpClient = httpClient;
//     }

//     public async Task<bool> VerifyRecaptchaAsync(string recaptchaToken)
//     {
//         var recaptchaSecret = _configuration["GoogleRecaptcha:SecretKey"];
        
//         if (string.IsNullOrEmpty(recaptchaSecret))
//         {
//             Console.WriteLine("Error: La clave secreta de reCAPTCHA no está configurada.");
//             return false;
//         }

//         if (string.IsNullOrEmpty(recaptchaToken))
//         {
//             Console.WriteLine("Error: El token de reCAPTCHA proporcionado es nulo o vacío.");
//             return false;
//         }

//         try
//         {
//             var response = await _httpClient.PostAsync(
//                 $"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaSecret}&response={recaptchaToken}", 
//                 null);

//             if (!response.IsSuccessStatusCode)
//             {
//                 Console.WriteLine($"Error en la solicitud a reCAPTCHA: Código de estado HTTP {response.StatusCode}");
//                 return false;
//             }

//             var result = await response.Content.ReadAsStringAsync();
            
//             // Deserializar la respuesta JSON
//             var jsonData = JsonSerializer.Deserialize<RecaptchaResponse>(result);

//             if (jsonData == null)
//             {
//                 Console.WriteLine("Error: No se pudo deserializar la respuesta de reCAPTCHA.");
//                 return false;
//             }

//             // Mostrar la respuesta completa para depuración
//             Console.WriteLine("Respuesta completa de reCAPTCHA: " + result);

//             if (!jsonData.Success)
//             {
//                 Console.WriteLine("Error: reCAPTCHA falló. Códigos de error:");
//                 if (jsonData.ErrorCodes != null && jsonData.ErrorCodes.Count > 0)
//                 {
//                     foreach (var errorCode in jsonData.ErrorCodes)
//                     {
//                         Console.WriteLine($"- {errorCode}");
//                     }
//                 }
//                 else
//                 {
//                     Console.WriteLine("- No se proporcionaron códigos de error.");
//                 }
//             }

//             return jsonData.Success;
//         }
//         catch (HttpRequestException ex)
//         {
//             Console.WriteLine("Error de red al comunicarse con el servicio de reCAPTCHA: " + ex.Message);
//             return false;
//         }
//         catch (JsonException ex)
//         {
//             Console.WriteLine("Error al procesar la respuesta JSON de reCAPTCHA: " + ex.Message);
//             return false;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine("Error inesperado: " + ex.Message);
//             return false;
//         }
//     }

//     private class RecaptchaResponse
//     {
//         public bool Success { get; set; }
//         public DateTime ChallengeTimestamp { get; set; }
//         public string Hostname { get; set; }
//         public List<string> ErrorCodes { get; set; }
//     }
// }













// // using System.Net.Http;
// // using System.Text.Json;
// // using System.Threading.Tasks;
// // using Microsoft.Extensions.Configuration;

// // public class RecaptchaService : IRecaptchaService
// // {
// //     private readonly IConfiguration _configuration;
// //     private readonly HttpClient _httpClient;

// //     public RecaptchaService(IConfiguration configuration, HttpClient httpClient)
// //     {
// //         _configuration = configuration;
// //         _httpClient = httpClient;
// //     }

// //     public async Task<bool> VerifyRecaptchaAsync(string recaptchaToken)
// //     {
// //         var recaptchaSecret = _configuration["GoogleRecaptcha:SecretKey"];
// //         var response = await _httpClient.PostAsync(
// //             $"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaSecret}&response={recaptchaToken}", 
// //             null);

// //         if (!response.IsSuccessStatusCode)
// //         {
// //             // Log error if the request to reCAPTCHA fails
// //             Console.WriteLine("Error en la solicitud a reCAPTCHA: " + response.StatusCode);
// //             return false;
// //         }

// //         var result = await response.Content.ReadAsStringAsync();
        
// //         // Deserialize the response using System.Text.Json
// //         var jsonData = JsonSerializer.Deserialize<RecaptchaResponse>(result);
        
// //         // Debug log: show the full response from reCAPTCHA
// //         Console.WriteLine("Respuesta de reCAPTCHA: " + result);  

// //         return jsonData != null && jsonData.Success;
// //     }

// //     private class RecaptchaResponse
// //     {
// //         public bool Success { get; set; }
// //         public float Score { get; set; }
// //         public string Action { get; set; }
// //     }
// // }





// // // using System.Net.Http;
// // // using System.Text.Json;
// // // using System.Threading.Tasks;
// // // using Microsoft.Extensions.Configuration;
// // // using System.Net.Http;

// // // public class RecaptchaService : IRecaptchaService
// // // {
// // //     private readonly IConfiguration _configuration;
// // //     private readonly HttpClient _httpClient;

// // //     public RecaptchaService(IConfiguration configuration, HttpClient httpClient)
// // //     {
// // //         _configuration = configuration;
// // //         _httpClient = httpClient;
// // //     }

// // //     public async Task<bool> VerifyRecaptchaAsync(string recaptchaToken)
// // //     {
// // //         var recaptchaSecret = _configuration["GoogleRecaptcha:SecretKey"];
// // //         var response = await _httpClient.PostAsync(
// // //             $"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaSecret}&response={recaptchaToken}", 
// // //             null);
            
// // //         var result = await response.Content.ReadAsStringAsync();

// // //         // Deserializar usando System.Text.Json
// // //         var jsonData = JsonSerializer.Deserialize<RecaptchaResponse>(result);
// // //         return jsonData != null && jsonData.Success;
// // //     }

// // //     private class RecaptchaResponse
// // //     {
// // //         public bool Success { get; set; }
// // //         public float Score { get; set; }
// // //         public string Action { get; set; }
// // //     }
// // // }

