using Firebase.Auth;
using Firebase.Storage;
using MBKC.Repository.FirebaseStorages.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.FirebaseStorages.Repositories
{
    public class FirebaseStorageRepository
    {
        public FirebaseStorageRepository()
        {

        }

        private FirebaseStorageModel GetFirebaseStorageProperties()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            return new FirebaseStorageModel()
            {
                ApiKey = configuration.GetSection("FirebaseStorage:apiKey").Value,
                Bucket = configuration.GetSection("FirebaseStorage:bucket").Value,
                AuthEmail = configuration.GetSection("FirebaseStorage:authEmail").Value,
                AuthPassword = configuration.GetSection("FirebaseStorage:authPassword").Value
            };
        }

        public async Task<string> UploadImageAsync(FileStream stream, string folder, string fileId)
        {
            try
            {
                FirebaseStorageModel firebaseStorage = GetFirebaseStorageProperties();
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(firebaseStorage.ApiKey));
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync(firebaseStorage.AuthEmail, firebaseStorage.AuthPassword);


                CancellationTokenSource cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    firebaseStorage.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(firebaseAuthLink.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(folder)
                    .Child(fileId)
                    .PutAsync(stream, cancellation.Token);

                string link = await task;
                return link;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task DeleteImageAsync(string fileId, string folder)
        {
            try
            {
                FirebaseStorageModel firebaseStorage = GetFirebaseStorageProperties();
                FirebaseAuthProvider firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(firebaseStorage.ApiKey));
                FirebaseAuthLink firebaseAuthLink = await firebaseAuthProvider.SignInWithEmailAndPasswordAsync(firebaseStorage.AuthEmail, firebaseStorage.AuthPassword);


                CancellationTokenSource cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    firebaseStorage.Bucket,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(firebaseAuthLink.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(folder)
                    .Child(fileId)
                    .DeleteAsync();
                await task;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}
