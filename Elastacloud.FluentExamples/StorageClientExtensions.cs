using System;
using System.Linq;
using System.Net;
using Elastacloud.AzureManagement.Fluent.Clients;

namespace Elastacloud.FluentExamples
{
	public static class StorageClientExtensions
	{
		/// <summary>
		/// If the subscription already contains a storage account by the same name - none will be created.
		/// </summary>
		/// <param name="storageClient">The StorageClient that mapps our subscription.</param>
		/// <param name="storageAccountName">The Storage Account we want to create.</param>
		/// <param name="location">The location where we want the storage account</param>
		/// <exception cref="InvalidOperationException">If the <paramref name="storageAccountName"/> is not globally unique</exception>
		public static bool CreateNewStorageAccountIfNotExists(this StorageClient storageClient, string storageAccountName, string location = "North Europe")
		{
			var storageAccountList = storageClient.GetStorageAccountList();
			if (storageAccountList.Any(sa => sa.Name == storageAccountName))
			{
				return false;
			}
			
			try
			{
				storageClient.CreateNewStorageAccount(storageAccountName, location);
			}
			catch (WebException we)
			{
				if (we.Status != WebExceptionStatus.ProtocolError ||
				    we.Message != "The remote server returned an error: (409) Conflict.")
				{
					throw new InvalidOperationException(string.Format("The storage account '{0}' already exists.", storageAccountName), we);
				}
				throw;
			}

			return true;
		}
	
		/// <summary>
		/// If the storage account already exists (have to be globally unique) - none will be created.
		/// </summary>
		/// <param name="storageClient">The StorageClient that mapps our subscription.</param>
		/// <param name="storageAccountName">The Storage Account we want to create.</param>
		/// <param name="location">The location where we want the storage account</param>
		/// <returns>True if the storage account was created. False if it was not created. It could already exist for instance.</returns>
		public static bool TryCreateNewStorageAccount(this StorageClient storageClient, string storageAccountName, string location = "North Europe")
		{
			try
			{
				return storageClient.CreateNewStorageAccountIfNotExists(storageAccountName, location);
			}
			catch (WebException)
			{
				return false;
			}
			catch (InvalidOperationException)
			{
				return false;
			}
		}
	}
}