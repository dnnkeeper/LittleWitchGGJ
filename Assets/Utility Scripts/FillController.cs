using UnityEngine;
using UnityEngine.Events;

public class FillController : MonoBehaviour
{
    public float targetFillAmount = 0f;

    public float fillStepAmount = 0.1f;

    public float fillSpeed = 10f;

    public Animator animator;

    public UnityEvent onFill;

    public UnityEvent<float> onProgress;

    public UnityEvent onStart;

    public UnityEvent onStall;

    [SerializeField]
    private float fillAmount = 0f;
    public float FillAmount
    {
        get { return fillAmount; }
        set
        {
            if (value != fillAmount)
            {
                fillAmount = Mathf.Clamp01(value);
                animator.SetFloat("FillAmount", fillAmount);
                if (fillAmount >= 1f)
                {
                    Debug.Log(gameObject.name + " FillAmount = " + fillAmount);
                    onFill.Invoke();
                }
                else
                {
                    onProgress.Invoke(fillAmount);
                }
            }
        }
    }

    public bool _inProgress;
    public bool inProgress
    {
        get { return _inProgress; }
        set
        {
            if (_inProgress != value)
            {
                if (value)
                {
                    onStart.Invoke();
                }
                else
                {
                    onStall.Invoke();
                }
                _inProgress = value;
            }
        }
    }

    public void SetTargetFillAmount(float value)
    {
        targetFillAmount = value;
    }

    void Fill()
    {
        //Debug.Log("FillWithTea()", gameObject);
        targetFillAmount = Mathf.Clamp01(targetFillAmount + fillStepAmount);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Fill controller has no animator component!");
            enabled = false;
        }
    }

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }



    float prevFillAmount;

    // Update is called once per frame
    void Update()
    {
        FillAmount = Mathf.Clamp01(Mathf.MoveTowards(FillAmount, targetFillAmount, Time.deltaTime * fillSpeed));
        if ((FillAmount - prevFillAmount) > Mathf.Epsilon)
        {
            inProgress = true;
        }
        else
        {
            inProgress = false;
        }
        prevFillAmount = FillAmount;
        //float diff = targetFillAmount - FillAmount;
        //if ( Mathf.Abs(diff) > Time.smoothDeltaTime)
        //    FillAmount = Mathf.Clamp01(FillAmount + Mathf.Sign(diff) * Time.deltaTime);
        //else
        //    FillAmount = targetFillAmount;
    }
}
