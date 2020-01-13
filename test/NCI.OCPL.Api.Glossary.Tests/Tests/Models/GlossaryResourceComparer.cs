using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests
{
  /// <summary>
  /// A IEqualityComparer for GlossaryResource
  /// </summary>
  public class GlossaryResourceComparer : IEqualityComparer<GlossaryResource>
  {
    public bool Equals(GlossaryResource x, GlossaryResource y)
    {
      // If the items are both null, or if one or the other is null, return
      // the correct response right away.

      if (x == null && y == null)
      {
        return true;
      }
      else if (x == null || y == null)
      {
        return false;
      }

      bool isEqual =
        x.Audience == y.Audience
        && x.Dictionary == y.Dictionary
        && x.Id == y.Id
        && x.PrettyUrlName == y.PrettyUrlName
        && x.Text == y.Text
        && x.Type == y.Type;

      return isEqual;
    }

    public int GetHashCode(GlossaryResource obj)
    {
      int hash = 0;
      hash ^=
        obj.Audience.GetHashCode()
        ^ (obj.Dictionary != null ? obj.Dictionary.GetHashCode() : 0)
        ^ obj.Id.GetHashCode()
        ^ (obj.PrettyUrlName != null ? obj.PrettyUrlName.GetHashCode() : 0)
        ^ obj.Text.GetHashCode()
        ^ obj.Type.GetHashCode();

      return hash;
    }
  }
}

