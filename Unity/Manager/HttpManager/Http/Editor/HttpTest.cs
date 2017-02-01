using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UniFramework.Extension;

[TestFixture]
[Category("Sample Tests")]
internal class HttpTests
{

	[Test]
	[Category("Url ExtendQuery Test")]
	public void URLExtendQueryTest()
	{
		Uri uri = new Uri("http://www.google.com/test?s=3&d=2");
		Dictionary<string,string> dic = new Dictionary<string,string>();
		dic.Add("a", "b"); 
		uri = uri.ExtendQuery(dic);
		Assert.AreEqual("http://www.google.com/test?s=3&d=2&a=b",uri.AbsoluteUri);
		dic.Add("t", "cc"); 
		uri = uri.ExtendQuery(dic);
		Assert.AreEqual("http://www.google.com/test?s=3&d=2&a=b&t=cc",uri.AbsoluteUri);
	}


//	[Test]
//	[Category("Failing Tests")]
//	public void ExceptionTest()
//	{
//		throw new Exception("Exception throwing test");
//	}
//
//	[Test]
//	[Ignore("Ignored test")]
//	public void IgnoredTest()
//	{
//		throw new Exception("Ignored this test");
//	}
//
//
//
//	[Test]
//	[Category("Failing Tests")]
//	public void FailingTest()
//	{
//		Assert.Fail();
//	}
//
//	[Test]
//	[Category("Failing Tests")]
//	public void InconclusiveTest()
//	{
//		Assert.Inconclusive();
//	}
//
//	[Test]
//	public void PassingTest()
//	{
//		Assert.Pass();
//	}
//
//	[Test]
//	public void ParameterizedTest([Values(1, 2, 3)] int a)
//	{
//		Assert.Pass();
//	}
//
//	[Test]
//	public void RangeTest([NUnit.Framework.Range(1, 10, 3)] int x)
//	{
//		Assert.Pass();
//	}
//
//	[Test]
//	[Culture("pl-PL")]
//	public void CultureSpecificTest()
//	{
//	}
//
//	[Test]
//	[ExpectedException(typeof(ArgumentException), ExpectedMessage = "expected message")]
//	public void ExpectedExceptionTest()
//	{
//		throw new ArgumentException("expected message");
//	}
//
//	[Datapoint]
//	public double zero = 0;
//	[Datapoint]
//	public double positive = 1;
//	[Datapoint]
//	public double negative = -1;
//	[Datapoint]
//	public double max = double.MaxValue;
//	[Datapoint]
//	public double infinity = double.PositiveInfinity;
//
//	[Theory]
//	public void SquareRootDefinition(double num)
//	{
//		Assume.That(num >= 0.0 && num < double.MaxValue);
//
//		var sqrt = Math.Sqrt(num);
//
//		Assert.That(sqrt >= 0.0);
//		Assert.That(sqrt * sqrt, Is.EqualTo(num).Within(0.000001));
//	}
}
