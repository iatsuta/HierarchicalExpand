using CommonFramework;
using CommonFramework.GenericRepository;

using HierarchicalExpand.DependencyInjection;
using HierarchicalExpand.Tests.Domain;

using Microsoft.Extensions.DependencyInjection;

namespace HierarchicalExpand.Tests;

public class DiTests
{
	[Fact]
	public async Task GetParents_ResultCorrect()
	{
		// Arrange
		var ct = TestContext.Current.CancellationToken;

		var queryableSource = Substitute.For<IQueryableSource>();
		queryableSource.GetQueryable<DomainObject>().Returns(_ => AllNodes.AsQueryable());
		queryableSource.GetQueryable<DirectAncestorLink>().Returns(_ => GetDirectAncestorLinks().AsQueryable());
		queryableSource.GetQueryable<UnDirectAncestorLink>().Returns(_ => GetUndirectAncestorLinks().AsQueryable());

		var rootSp = new ServiceCollection()
			.AddScoped(_ => queryableSource)
			.AddScoped(_ => Substitute.For<IGenericRepository>())
			.AddHierarchicalExpand(s => s.AddHierarchicalInfo(
				v => v.Parent,
				new AncestorLinkInfo<DomainObject, DirectAncestorLink>(l => l.From, l => l.To),
				new AncestorLinkInfo<DomainObject, UnDirectAncestorLink>(l => l.From, l => l.To)))
			.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

		await using var scope = rootSp.CreateAsyncScope();

		var sp = scope.ServiceProvider;

		var service = sp.GetRequiredService<IHierarchicalObjectExpanderFactory>();

		// Act
		var result = service.Create<int>(typeof(DomainObject)).Expand([3, 13], HierarchicalExpandType.Parents).Order();

		// Assert
		result.Should().BeEquivalentTo([0, 1, 2, 3, 12, 13]);
	}

	private static IEnumerable<DirectAncestorLink> GetDirectAncestorLinks()
	{
		return

			from node in AllNodes

			from parentNode in node.GetAllElements(n => n.Parent)

			select new DirectAncestorLink { From = parentNode, To = node };
	}

	private static IEnumerable<UnDirectAncestorLink> GetUndirectAncestorLinks()
	{
		return GetDirectAncestorLinks()
			.SelectMany(link => new[]
				{ new UnDirectAncestorLink { From = link.From, To = link.To }, new UnDirectAncestorLink { From = link.To, To = link.From } })
			.Distinct();
	}

	private static IEnumerable<DomainObject> BuildTree()
	{
		var root = new DomainObject { Name = "Root", Id = 0 };

		var a = new DomainObject { Name = "A", Parent = root, Id = 1 };
		var a1 = new DomainObject { Name = "A1", Parent = a, Id = 2 };
		var a1A = new DomainObject { Name = "A1a", Parent = a1, Id = 3 };
		var a1A1 = new DomainObject { Name = "A1a1", Parent = a1A, Id = 4 };
		var a1A1X = new DomainObject { Name = "A1a1x", Parent = a1A1, Id = 5 };
		var a1A2 = new DomainObject { Name = "A1a2", Parent = a1A, Id = 6 };
		var a1B = new DomainObject { Name = "A1b", Parent = a1, Id = 7 };
		var a1B1 = new DomainObject { Name = "A1b1", Parent = a1B, Id = 8 };
		var a1B1X = new DomainObject { Name = "A1b1x", Parent = a1B1, Id = 9 };
		var a2 = new DomainObject { Name = "A2", Parent = a, Id = 10 };
		var a2A = new DomainObject { Name = "A2a", Parent = a2, Id = 11 };

		var b = new DomainObject { Name = "B", Parent = root, Id = 12 };
		var b1 = new DomainObject { Name = "B1", Parent = b, Id = 13 };
		var b1A = new DomainObject { Name = "B1a", Parent = b1, Id = 14 };
		var b1A1 = new DomainObject { Name = "B1a1", Parent = b1A, Id = 15 };
		var b1B = new DomainObject { Name = "B1b", Parent = b1, Id = 16 };
		var b1B1 = new DomainObject { Name = "B1b1", Parent = b1B, Id = 17 };
		var b1B1X = new DomainObject { Name = "B1b1x", Parent = b1B1, Id = 18 };
		var b1B2 = new DomainObject { Name = "B1b2", Parent = b1B, Id = 19 };
		var b2 = new DomainObject { Name = "B2", Parent = b, Id = 20 };

		var c = new DomainObject { Name = "C", Parent = root, Id = 21 };
		var c1 = new DomainObject { Name = "C1", Parent = c, Id = 22 };
		var c1A = new DomainObject { Name = "C1a", Parent = c1, Id = 23 };
		var c1A1 = new DomainObject { Name = "C1a1", Parent = c1A, Id = 24 };
		var c1B = new DomainObject { Name = "C1b", Parent = c1, Id = 25 };
		var c1B1 = new DomainObject { Name = "C1b1", Parent = c1B, Id = 26 };
		var c2 = new DomainObject { Name = "C2", Parent = c, Id = 27 };

		return
		[
			root,
			a, a1, a1A, a1A1, a1A1X, a1A2, a1B, a1B1, a1B1X, a2, a2A,
			b, b1, b1A, b1A1, b1B, b1B1, b1B1X, b1B2, b2,
			c, c1, c1A, c1A1, c1B, c1B1, c2
		];
	}

	private static readonly IReadOnlyList<DomainObject> AllNodes = BuildTree().ToArray();
}