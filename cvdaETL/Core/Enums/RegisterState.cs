using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cvdaETL.Core.Enums
{
    public enum RegisterState
    {
        Active,
        New,
        ConditionExclusions,
        FollowUp,
        Seen,
        DNA,
        Died,
        RecallExclusions,
    }
}

/// Key is as follows
/// AcitveNotSeen = 1 => New on the list but not seen yet
/// New = 2 => New on the list
/// Removed = 3 => No longer on the base list
/// FollowUp = 4 => Need to be followed up (seen in clinic and need to be followed up)
/// Seen = 5 => Seen in clinic
/// DNA = 6 => Did not attend clinic
/// Died = 7 => Died
/// Excluded = 8 => Excluded from the list (via excluded.csv)
/// RecallExcluded = 9 => Excluded from the list (via recall.csv)
/// TriagedExcluded = 10 => Excluded from the list (via Inappropriate Triage)
/// 
