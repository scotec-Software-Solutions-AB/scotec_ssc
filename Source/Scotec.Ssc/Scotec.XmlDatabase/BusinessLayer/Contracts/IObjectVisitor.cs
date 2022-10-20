using System;

namespace Scotec.XMLDatabase
{
	/// <summary>
	/// Summary description for IDefaultVisitor.
	/// </summary>
	public interface IObjectVisitor : IVisitor
	{
		object Visit(object visitable);
	}

	public interface IObjectVisitor<T> : IVisitor<T>
	{
		T Visit(object visitable);
	}
}
