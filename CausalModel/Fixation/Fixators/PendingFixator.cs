using CausalModel.Model.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CausalModel.Fixation.Fixators
{
    public delegate void FactPendingEventHandler<TFactValue>(
        object sender,
        InstanceFact<TFactValue> pendingFactId,
        bool isOccurred)
        where TFactValue : class;

    /// <summary>
    /// Fixator developed for some scenarios of real time causal generation.
    /// In addition to the usual fixation called from the generator,
    /// facts must also be approved by an external code to be finally fixated
    /// </summary>
    public class PendingFixator<TFactValue> : Fixator<TFactValue>
        where TFactValue : class
    {
        private enum PendingFactFixationStatus
        {
            NoneToPending = 0,  // The fact occurs for the first time during fixation
            PendingToApprove = 1,
            ApprovedToFixate = 2,
            Fixated = 3,
        }

        private Dictionary<InstanceFactId, bool> pendingFacts
            = new Dictionary<InstanceFactId, bool>();
        private Dictionary<InstanceFactId, bool> factsApprovedToFixate
            = new Dictionary<InstanceFactId, bool>();
        public event FactPendingEventHandler<TFactValue>? FactPending;

        private int approvePendingFactsCallsCount = 0;
        public int ApprovePendingFactsCallsCount => approvePendingFactsCallsCount;

        public virtual bool ShouldBePending(InstanceFact<TFactValue> fact) => true;

        public override void HandleFixation(
            InstanceFact<TFactValue> fact,
            bool isOccurred)
        {
            switch (GetFactStatus(fact))
            {
                case PendingFactFixationStatus.NoneToPending:
                    LogInfo("Fact occurred the first time : " + fact + ", "
                        + isOccurred);
                    AddPendingFact(fact, isOccurred);
                    break;
                case PendingFactFixationStatus.ApprovedToFixate:
                    LogInfo("Fact approved to fixate : " + fact + ", "
                        + isOccurred);
                    Fixate(fact.InstanceFactId, isOccurred);
                    break;
                case PendingFactFixationStatus.PendingToApprove:
                    LogWarning("[WRN] Fixation of a pending fact: " + fact + ", "
                        + isOccurred);
                    break;
                case PendingFactFixationStatus.Fixated:
                    LogWarning("[WRN] Repeated fixation of a fact: " + fact + ", "
                        + isOccurred);
                    break;
                default:
                    throw new InvalidOperationException("Not supported " +
                        "fact fixation status");
            }
        }

        public bool HasPendingFacts() => pendingFacts.Count > 0;

        public void ApproveFixation(InstanceFactId factId, bool? isOccurred = null)
        {
            if (!pendingFacts.ContainsKey(factId))
                throw new InvalidOperationException($"Fact not pending to fixation "
                    + "cannot be approved");

            var fact = pendingFacts[factId];
            pendingFacts.Remove(factId);
            factsApprovedToFixate.Add(factId, fact);

            Fixate(factId, isOccurred ?? fact);
        }
        
        public void ApprovePendingFacts()
        {
            LogInfo("Approve pending facts calls count: " +
                ++approvePendingFactsCallsCount);

            var factIds = pendingFacts.Keys.ToList();
            foreach (var instanceFactId in factIds)
            {
                ApproveFixation(instanceFactId);
            }
        }

        private void AddPendingFact(InstanceFact<TFactValue> fact, bool isOccurred)
        {
            if (pendingFacts.ContainsKey(fact.InstanceFactId))
            {
                LogWarning("[WRN] Fact is already pending: " + fact + ", "
                    + isOccurred);
                return;
            }
            pendingFacts.Add(fact.InstanceFactId, isOccurred);
            FactPending?.Invoke(this, fact, isOccurred);
        }

        private PendingFactFixationStatus GetFactStatus(
            InstanceFact<TFactValue> fact)
        {
            var factId = fact.InstanceFactId;
            if (factsApprovedToFixate.ContainsKey(factId))
                return PendingFactFixationStatus.ApprovedToFixate;
            else if (pendingFacts.ContainsKey(factId))
                return PendingFactFixationStatus.PendingToApprove;
            else if (IsFixated(factId) != null)
                return PendingFactFixationStatus.Fixated;
            else
            {
                return ShouldBePending(fact)
                    ? PendingFactFixationStatus.NoneToPending
                    : PendingFactFixationStatus.ApprovedToFixate;
            }
        }

        #region Logging
        // Todo: actual logging implementation
        private void LogInfo(string str)
        {
            //WriteColoredText(str, ConsoleColor.DarkGray);
        }
        private void LogWarning(string str)
        {
            //WriteColoredText(str, ConsoleColor.Yellow);
        }

        private void WriteColoredText(string str, ConsoleColor color)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(str);
            Console.ForegroundColor = prevColor;
        }
        #endregion
    }
}
