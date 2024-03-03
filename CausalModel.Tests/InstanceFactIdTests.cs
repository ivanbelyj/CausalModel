using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CausalModel.Tests;
public class InstanceFactIdTests
{
    [Fact]
    public void EqualsTest()
    {
        var id1 = new InstanceFactId("fact1", "instance1");
        var id2 = new InstanceFactId("fact1", "instance1");
        var id3 = new InstanceFactId("fact2", "instance2");

        Assert.Equal(id1, id2);
        Assert.True(id1 == id2);

        Assert.NotEqual(id1, id3);
        Assert.True(id1 != id3);

        var address = id1.ToAddress();
        Assert.Equal(id1.ModelInstanceId, address.ModelInstanceId);
        Assert.Equal(id1.FactId, address.FactId);
    }
}
