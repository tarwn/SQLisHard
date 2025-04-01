using Moq;
using NUnit.Framework;
using SQLisHard.General.ExperienceLogging.Communications;
using SQLisHard.General.ExperienceLogging.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SQLisHard.General.tests.ExperienceLogging.Log
{
	[TestFixture]
	public class MultiProviderTests
	{
		private Result GetSuccessResult()
		{
			return new Result() { Success = true };
		}

		private Result GetErrorResult()
		{
			return new Result() { Success = false };
		}

		[Test]
		public void Constructor_InitializesEmptyList()
		{
			var multiProvider = new MultiProvider();
			// expect no exception to occur
		}

		[Test]
		public void Log_NoProvidersAssigned_ReportsSuccess() 
		{
			var provider = new MultiProvider();
			var sampleMessage = new Dictionary<string, object>();
			var actualResult = GetErrorResult();	// start with error to be sure the Result is updated

			provider.Log(sampleMessage, (result) => { actualResult = result; });

			Assert.That(actualResult.Success, Is.True);
		}

		[Test]
		public void Log_SingleProviderWithSuccessfulResult_ReportsSuccess()
		{
			var provider = new MultiProvider();
			var sampleMessage = new Dictionary<string, object>();
			var fakeProvider = new Mock<ILogProvider>();
			fakeProvider.Setup(p => p.Log(It.IsAny<Dictionary<string, object>>(), It.IsAny<Action<Result>>()))
						.Callback<Dictionary<string, object>, Action<Result>>((msg, callback) => callback(GetSuccessResult()));
			provider.AddProvider(fakeProvider.Object);
			var actualResult = GetErrorResult();	// start with error to be sure the Result is updated

			provider.Log(sampleMessage, (result) => { actualResult = result; });

			Assert.That(actualResult.Success, Is.True);
		}

		[Test]
		public void Log_SingleProviderWithUnsuccessfulResult_ReportsUnsuccessfulResult()
		{
			var provider = new MultiProvider();
			var sampleMessage = new Dictionary<string, object>();
			var fakeProvider = new Mock<ILogProvider>();
			fakeProvider.Setup(p => p.Log(It.IsAny<Dictionary<string, object>>(), It.IsAny<Action<Result>>()))
						.Callback<Dictionary<string, object>, Action<Result>>((msg, callback) => callback(GetErrorResult()));
			provider.AddProvider(fakeProvider.Object);
			var actualResult = GetSuccessResult();

			provider.Log(sampleMessage, (result) => { actualResult = result; });

			Assert.That(actualResult.Success, Is.False);
		}

		[Test]
		public void Log_TwoProvidersWithSuccessfulResults_ReportsSuccess()
		{
			var provider = new MultiProvider();
			var sampleMessage = new Dictionary<string, object>();
			var fakeProvider = new Mock<ILogProvider>();
			fakeProvider.Setup(p => p.Log(It.IsAny<Dictionary<string, object>>(), It.IsAny<Action<Result>>()))
						.Callback<Dictionary<string, object>, Action<Result>>((msg, callback) => callback(GetSuccessResult()));
			provider.AddProvider(fakeProvider.Object);
			fakeProvider = new Mock<ILogProvider>();
			fakeProvider.Setup(p => p.Log(It.IsAny<Dictionary<string, object>>(), It.IsAny<Action<Result>>()))
						.Callback<Dictionary<string, object>, Action<Result>>((msg, callback) => callback(GetSuccessResult()));
			provider.AddProvider(fakeProvider.Object);
			var actualResult = GetErrorResult();	// start with error to be sure the Result is updated

			provider.Log(sampleMessage, (result) => { actualResult = result; });

			Assert.That(actualResult.Success, Is.True);
		}

		[Test]
		public void Log_TwoProvidersWithUnsuccessfulResults_ReportsUnsuccessfulResult()
		{
			var provider = new MultiProvider();
			var sampleMessage = new Dictionary<string, object>();
			var fakeProvider = new Mock<ILogProvider>();
			fakeProvider.Setup(p => p.Log(It.IsAny<Dictionary<string, object>>(), It.IsAny<Action<Result>>()))
						.Callback<Dictionary<string, object>, Action<Result>>((msg, callback) => callback(GetErrorResult()));
			provider.AddProvider(fakeProvider.Object);
			fakeProvider = new Mock<ILogProvider>();
			fakeProvider.Setup(p => p.Log(It.IsAny<Dictionary<string, object>>(), It.IsAny<Action<Result>>()))
						.Callback<Dictionary<string, object>, Action<Result>>((msg, callback) => callback(GetErrorResult()));
			provider.AddProvider(fakeProvider.Object);
			var actualResult = GetSuccessResult();

			provider.Log(sampleMessage, (result) => { actualResult = result; });

			Assert.That(actualResult.Success, Is.False);
		}

		[Test]
		public void Log_TwoProvidersWithSuccesfulAndUnsuccessfulResults_ReportsUnsuccessfulResult()
		{
			var provider = new MultiProvider();
			var sampleMessage = new Dictionary<string, object>();
			var fakeProvider = new Mock<ILogProvider>();
			fakeProvider.Setup(p => p.Log(It.IsAny<Dictionary<string, object>>(), It.IsAny<Action<Result>>()))
						.Callback<Dictionary<string, object>, Action<Result>>((msg, callback) => callback(GetSuccessResult()));
			provider.AddProvider(fakeProvider.Object);
			fakeProvider = new Mock<ILogProvider>();
			fakeProvider.Setup(p => p.Log(It.IsAny<Dictionary<string, object>>(), It.IsAny<Action<Result>>()))
						.Callback<Dictionary<string, object>, Action<Result>>((msg, callback) => callback(GetErrorResult()));
			provider.AddProvider(fakeProvider.Object);
			var actualResult = GetSuccessResult();

			provider.Log(sampleMessage, (result) => { actualResult = result; });

			Assert.That(actualResult.Success, Is.False);
		}

		[Test]
		public void Log_TwoProvidersWithUnsuccessfulAndSuccessfulResults_ReportsUnsuccessfulResult()
		{
			var provider = new MultiProvider();
			var sampleMessage = new Dictionary<string, object>();
			var fakeProvider = new Mock<ILogProvider>();
			fakeProvider.Setup(p => p.Log(It.IsAny<Dictionary<string, object>>(), It.IsAny<Action<Result>>()))
						.Callback<Dictionary<string, object>, Action<Result>>((msg, callback) => callback(GetErrorResult()));
			provider.AddProvider(fakeProvider.Object);
			fakeProvider = new Mock<ILogProvider>();
			fakeProvider.Setup(p => p.Log(It.IsAny<Dictionary<string, object>>(), It.IsAny<Action<Result>>()))
						.Callback<Dictionary<string, object>, Action<Result>>((msg, callback) => callback(GetSuccessResult()));
			provider.AddProvider(fakeProvider.Object);
			var actualResult = GetSuccessResult();

			provider.Log(sampleMessage, (result) => { actualResult = result; });

			Assert.That(actualResult.Success, Is.False);
		}

		[Test]
		public void Log_NoProvidersAssignedAndNoCallback_DoesNotBlowUp()
		{
			var provider = new MultiProvider();
			var sampleMessage = new Dictionary<string, object>();

			provider.Log(sampleMessage, null);

			// expect no exception to occur
			Assert.Pass();
		}

		[Test]
		public void Log_SingleProviderWithoutCallback_DoesNotBlowUp()
		{
			var provider = new MultiProvider();
			var sampleMessage = new Dictionary<string, object>();
			var fakeProvider = new Mock<ILogProvider>();
			fakeProvider.Setup(p => p.Log(It.IsAny<Dictionary<string, object>>(), It.IsAny<Action<Result>>()))
						.Callback<Dictionary<string, object>, Action<Result>>((msg, callback) => {
							if (callback != null) { callback(GetSuccessResult()); }
						});
			provider.AddProvider(fakeProvider.Object);

			provider.Log(sampleMessage, null);

			// expect no exception to occur
		}
	}
}
