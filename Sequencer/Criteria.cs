using UnityEngine;
using System.Collections.Generic;
using fastJSON;

//general purpose criteria
public abstract class Criteria {
	public enum Eval {
		Less,
		Greater,
		Equal,
		NotEqual,
		LessEqual,
		GreaterEqual,
		False,
		True,
		
		NumEvals
	}
	
	public static bool Check<T>(T lhs, T rhs, Eval eval, IComparer<T> comparer) {
		switch(eval) {
		case Eval.Less:
			return comparer.Compare(lhs, rhs) < 0;
		case Eval.Greater:
			return comparer.Compare(lhs, rhs) > 0;
		case Eval.Equal:
		case Eval.True:
			return comparer.Compare(lhs, rhs) == 0;
		case Eval.NotEqual:
		case Eval.False:
			return comparer.Compare(lhs, rhs) != 0;
		case Eval.LessEqual:
			return comparer.Compare(lhs, rhs) <= 0;
		case Eval.GreaterEqual:
			return comparer.Compare(lhs, rhs) <= 0;
		}
		
		return false;
	}
	
	public static Eval ToEval(string e) {
		if(!string.IsNullOrEmpty(e)) {
			switch(e) {
			case "<": return Eval.Less;
			case ">": return Eval.Greater;
			case "=": case "==": case "is": return Eval.Equal;
			case "!=": return Eval.NotEqual;
			case "<=": return Eval.LessEqual;
			case ">=": return Eval.GreaterEqual;
			case "false": case "!": return Eval.False;
			}
		}
		
		return Eval.True;
	}
	
	//fill this in json
	public string e = ""; //true
    public object val;
	
	public Eval eval {
		get { return mEval; }
	}
	
	private Eval mEval = Eval.True;
	
	public static Criteria[] LoadCriterias(string data) {
		JSON.Instance.Parameters.UseExtensions = true;
		
		List<Criteria> criterias = JSON.Instance.ToObject<List<Criteria>>(data);
		
		return criterias.ToArray();
	}
	
	public static bool EvaluateCriterias(Criteria[] criterias, Object param) {
		foreach(Criteria criteria in criterias) {
			if(criteria.Evaluate(param)) {
				return true;
			}
		}
		
		return false;
	}
	
	public Criteria() {
		mEval = ToEval(e);
	}

    public bool Evaluate(Object param) {
        switch(eval) {
            case Eval.Less:
                return DoCompare(param, val) < 0;
            case Eval.Greater:
                return DoCompare(param, val) > 0;
            case Eval.Equal:
            case Eval.True:
                return DoCompare(param, val) == 0;
            case Eval.NotEqual:
            case Eval.False:
                return DoCompare(param, val) != 0;
            case Eval.LessEqual:
                return DoCompare(param, val) <= 0;
            case Eval.GreaterEqual:
                return DoCompare(param, val) <= 0;
        }

        return false;
    }
	
	//implements

    protected abstract int DoCompare(Object param, object val);
}
