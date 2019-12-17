using System.Linq;
using System.Collections.Generic;
using NCI.OCPL.Api.Glossary;

namespace NCI.OCPL.Api.BestBets.Tests
{
    /// <summary>
    /// A IEqualityComparer for GlossaryTerm
    /// </summary>
    public class GlossaryTermComparer : IEqualityComparer<GlossaryTerm>
    {
        public bool Equals(GlossaryTerm x, GlossaryTerm y)
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
                x.Id == y.Id
                && x.Language == y.Language
                && x.Dictionary.ToLower() == y.Dictionary.ToLower()
                && x.Audience.ToString() == y.Audience.ToString();

            return isEqual;
        }

        public int GetHashCode(GlossaryTerm obj)
        {
            int hash = 0;
            hash ^=
                obj.Id.GetHashCode()
                ^ obj.Language.GetHashCode()
                ^ obj.Dictionary.GetHashCode()
                ^ (obj.Audience.ToString()).GetHashCode();

            return hash;
        }

        /// <summary>
        /// Helper function to determine param arrays are equal, order does not matter.
        /// </summary>
        /// <param name="x">Param array 1</param>
        /// <param name="y">Param array 2</param>
        /// <returns></returns>
        private bool AreParamArraysEqual(string[] x, string[] y)
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

            if (x.Count() != y.Count())
            {
                return false;
            }

            //Generate a set of those values that are not in both lists.
            //if this is not 0, then there is an error.
            var diffxy = x.Except(y);

            return diffxy.Count() == 0;
        }
    }
}