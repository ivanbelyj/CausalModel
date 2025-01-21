using CausalModel.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace CausalModel.Common.DataProviders
{
    // Todo: refactor / move exceptions logic
    public class ConventionsProvider<TFactValue> : IConventionsProvider
        where TFactValue : class
    {
        private Dictionary<string, BlockConvention>? conventionByName;
        private Dictionary<string, BlockCausesConvention>? causesConventionByName;

        public ConventionsProvider(
            IEnumerable<BlockConvention> conventions,
            IEnumerable<BlockCausesConvention> causesConventions)
        {
            SetBlockConventions(conventions);
            SetBlockCausesConventions(causesConventions);
        }

        public BlockConvention GetConventionByName(string convName)
        {
            if (conventionByName == null)
            {
                throw new InvalidOperationException("Cannot get convention "
                    + "from model that has no conventions");
            }
            return conventionByName[convName];
        }

        public BlockCausesConvention GetCauseConventionByName(string convName)
        {
            if (causesConventionByName == null)
            {
                throw new InvalidOperationException("Cannot get convention "
                    + "from model that has no causes conventions");
            }

            return causesConventionByName[convName];
        }

        private void SetBlockConventions(IEnumerable<BlockConvention>? conventions)
        {
            if (conventions != null)
            {
                conventionByName = new Dictionary<string, BlockConvention>();
                foreach (var conv in conventions)
                {
                    if (conv.Name == null)
                    {
                        throw new ArgumentException("Block convention name is required.");
                    }

                    conventionByName.Add(conv.Name, conv);
                }
            }
            else
            {
                conventionByName = null;
            }
        }

        private void SetBlockCausesConventions(IEnumerable<BlockCausesConvention>? conventions)
        {
            if (conventions != null)
            {
                causesConventionByName = new Dictionary<string, BlockCausesConvention>();
                foreach (var conv in conventions)
                {
                    if (conv.Name == null)
                        throw new ArgumentException("Block causes convention name "
                            + "is required.");
                    causesConventionByName.Add(conv.Name, conv);
                }
            }
            else
            {
                causesConventionByName = null;
            }
        }
    }
}
