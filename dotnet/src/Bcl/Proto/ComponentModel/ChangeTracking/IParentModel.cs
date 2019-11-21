
using System.Collections.Generic;
using NerdyMishka.ComponentModel.ChangeTracking;

public interface IParentModel
{
    IEnumerable<IChildModel> Children { get; }

    void Attach(IChildModel model);

    void Detach(IChildModel model);
}

public interface IChildModel : IChangeModel
{
    IParentModel Parent { get; set; }
}