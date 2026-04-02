using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

//熊怪,纯粹的进攻怪兽
public class ZomBear : EnemyBase
{
    //狂暴状态
    public float detectRange = 8f;
    public float enrageSpeed=1.5f;
    public float enrageDamage = 30f;

    //失心状态
    public float madnessSpeedMulti=3f;  // 移速+3
    public float madnessDuration = 10f;     // 持续10秒
    private float _madnessTimer;             // 计时
    private bool _isMadness;                 // 是否失心
    private bool _canBeBuffed = true;        // 防重复叠加

    // 状态机实例
    private FSM fsm;

    // 声明所有状态
    private BearIdleState idleState;
    private BearChaseState chaseState;
    private BearDeathState deathState;

    //受蛊惑(失心)
    public void ApplyMadness()
    {
        // 不可重复叠加
        if (!_canBeBuffed || isDead) return;

        _isMadness = true;
        _canBeBuffed = false;
        _madnessTimer = 0;
    }

    protected override void Awake()
    {
        base.Awake(); // 执行父类初始化

        // 初始化FSM
        fsm = new FSM();
        idleState = new BearIdleState(this);
        chaseState = new BearChaseState(this);
        deathState = new BearDeathState(this);
    }

    private void Start()
    {
        fsm.SwitchState(idleState);
    }

    //重写Update()
    protected override void Update()
    {
        if (isDead)
        {
            base.Update();
            return;
        }
        attackTimer += Time.deltaTime;

        // 失心BUFF计时
        if (_isMadness)
        {
            _madnessTimer += Time.deltaTime;
            if (_madnessTimer >= madnessDuration)
            {
                // 失心结束
                _isMadness = false;
                _canBeBuffed = true;
            }
        }
        fsm.Update();
    }

    protected override void Die()
    {
        fsm.SwitchState(deathState);
        base.Die();
    }

    //***三个状态***

    //待机状态
    private class BearIdleState :IState {

        private readonly ZomBear _bear;
        public BearIdleState(ZomBear bear) => _bear = bear;
        public void Enter()
        {
            _bear.Idle(); // 父类待机
            _bear.anim.SetBool("IsMoving", false);
        }
        public void Update()
        {
            float dis = Vector3.Distance(_bear.transform.position, _bear.targetPlayer.position);
            if (dis < _bear.detectRange)
            {
                _bear.fsm.SwitchState(_bear.chaseState);
            }

        }
        public void Exit()
        {

        }

    }


    //追击状态
    private class BearChaseState : IState {

        private readonly ZomBear _bear;
        public BearChaseState(ZomBear bear) => _bear = bear;
        public void Enter()
        {
            _bear.anim.SetBool("IsMoving", true);
        }
        public void Update()
        {
            float finalSpeed = _bear.moveSpeed;
            //狂暴
            if (_bear.currentHealth < _bear.maxHealth * 0.5f)
            {

                finalSpeed *= _bear.enrageSpeed;
                _bear.attackDamage = _bear.enrageDamage;
            }
            
            //失心
            if (_bear._isMadness)
            {
                Debug.Log("进入失心");
                finalSpeed += _bear.madnessSpeedMulti;
            }

            _bear.agent.speed = finalSpeed;
            _bear.ChasePlayer();

            float dis = Vector3.Distance(_bear.transform.position, _bear.targetPlayer.position);


            if (_bear.playerInRange && _bear.attackTimer >= _bear.timeBetweenAttacks)
            {
                _bear.Attack();
            }
        }
        public void Exit()
        {

        }
    }


    private class BearDeathState : IState
    {

        private readonly ZomBear _bear;
        public BearDeathState(ZomBear bear) => _bear = bear;
        public void Enter()
        {

        }
        public void Update()
        {

        }
        public void Exit()
        {

        }
    }

    private void OnEnable()
    {
        GameEvent.OnGunShot += OnGunShotHeard;
    }

    private void OnDisable()
    {
        GameEvent.OnGunShot -= OnGunShotHeard;
    }

    // 听到枪声 → 直接追玩家
    private void OnGunShotHeard()
    {
        if (!isDead && fsm.CurrentState != chaseState)
        {
            fsm.SwitchState(chaseState);
        }
    }


}