using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Инициализация проекта Firebase со стандартными правами доступа
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
            });
            // Инициализация сущности базы данных с ID базы данных
            FirestoreDb db = FirestoreDb.Create(@"akebia-9b123");
            // Инициализация Коллекии и получение всех документов в коллекции
            CollectionReference collection = db.Collection("Invoices");
            await Read(collection);
            Console.WriteLine(new string('-', 20));
            // Добавление новой записи
            // В случае, если она уже существует, ничего не поменяется.
            Add(db);
            await Read(collection);
            Console.ReadLine();
        }

        static async Task Read(CollectionReference collection)
        {
            QuerySnapshot snapshot = await collection.GetSnapshotAsync();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                await Console.Out.WriteLineAsync($"\n{document.Id}");
                Console.ForegroundColor = ConsoleColor.White;
                // Чтение каждой строки из каждого документа
                Dictionary<string, object> data = document.ToDictionary();

                foreach (KeyValuePair<string, object> pair in data)
                {
                    await Console.Out.WriteLineAsync($"[{pair.Key}]: {pair.Value}");
                }
            }
        }
        static void Add(FirestoreDb db, string docName = "example", string json = "")
        {

            DocumentReference docRef = db.Collection("Invoices").Document(docName);
            if (String.IsNullOrWhiteSpace(json))
                json = @"{
                    ""Arrival"": ""20 Apr 2023 at 02:25:21 UTC+7"",
                    ""Cost"": 1232216,
                    ""Departure"": ""24 Jan 2023 at 12:50:24 UTC+7"",
                    ""Value"": ""34000"" 
                }";
            Dictionary<string, object> user = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            // Асинхронное добавление документа в таблицу базы данных
            docRef.SetAsync(user);
        }
    }
}