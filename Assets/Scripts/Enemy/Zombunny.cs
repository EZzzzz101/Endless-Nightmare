using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

//兔子怪，怪叫似乎能迷惑熊怪为其赴死【为队友提供buff:{献身}]
public class Zombunny : EnemyBase
{

    public float detectRange = 4f;       // 极小警戒范围
    public float fleeSpeed = 6f;         // 逃跑速度
    public float cryEffectRange = 8f;     // 蛊惑熊的范围
    public float fleeRunDistance = 15f;   // 逃跑距离
    public AudioClip crySound;            // 哭泣音效
    private bool _hasFleeForever = false; //哭泣标记
    private bool hasPlayedCrySound = false; //哭泣开关

    // 状态机
    private FSM fsm;
    //声明状态
    private BunnyIdleState idleState;
    private BunnyChaseState chaseState;
    private BunnyFleeState fleeState;     // 逃跑+哭泣状态
    private BunnyDeathState deathState;

    private Vector3 fleeTargetPos;        // 逃跑目标点


    protected override void Awake()
    {
        base.Awake();
        fsm = new FSM();
        idleState = new BunnyIdleState(this);
        chaseState = new BunnyChaseState(this);
        fleeState = new BunnyFleeState(this);
        deathState = new BunnyDeathState(this);
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
        fsm.Update();

        // 只在【没有永久逃跑】的情况下，才会残血逃跑
        if (!_hasFleeForever && currentHealth < maxHealth * 0.5f && fsm.CurrentState != fleeState)
        {
            fsm.SwitchState(fleeState);
        }
    }

    public void SetRandomEscapeDir()
    {
        Vector3 awayDir = (transform.position - targetPlayer.position).normalized;
        fleeTargetPos = transform.position + awayDir * fleeRunDistance;
    }

    // 核心：哭泣蛊惑，给熊上献身标记
    public void CryToControlBears()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, cryEffectRange);
        foreach (var col in cols)
        {
            ZomBear bear = col.GetComponent<ZomBear>();
            if (bear != null)
            {
                bear.ApplyMadness();  // 兔子蛊惑熊进入失心
            }
        }
    }

    protected override void Die()
    {
        fsm.SwitchState(deathState);
        base.Die();
    }

    // 待机
    private class BunnyIdleState : IState
    {
        private readonly Zombunny _bunny;
        public BunnyIdleState(Zombunny bunny) => _bunny = bunny;

        public void Enter()
        {
            _bunny.Idle();
            _bunny.anim.SetBool("IsMoving", false);
            _bunny.anim.SetBool("IsMovingAway", false);
        }

        public void Update()
        {
            float dis = Vector3.Distance(_bunny.transform.position, _bunny.targetPlayer.position);
            // 只有没永久逃跑，才会追玩家
            if (!_bunny._hasFleeForever&&dis < _bunny.detectRange)
            {
                _bunny.fsm.SwitchState(_bunny.chaseState);
            }
        }

        public void Exit() { }
    }

    // 追击
    private class BunnyChaseState : IState
    {
        private readonly Zombunny _bunny;
        public BunnyChaseState(Zombunny bunny) => _bunny = bunny;

        public void Enter()
        {
            _bunny.anim.SetBool("IsMoving", true);
        }

        public void Update()
        {
            _bunny.agent.speed = _bunny.moveSpeed;
            _bunny.agent.SetDestination(_bunny.targetPlayer.position);


            if (_bunny.playerInRange && _bunny.attackTimer >= _bunny.timeBetweenAttacks)
            {
                _bunny.Attack();
            }
        }

        public void Exit() {
            
        }
    }

    // 逃跑 + 哭泣蛊惑（核心状态）
    private class BunnyFleeState : IState
    {
        private readonly Zombunny _bunny;
        public BunnyFleeState(Zombunny bunny) => _bunny = bunny;

        public void Enter()
        {
            _bunny._hasFleeForever = true;
            _bunny.anim.SetBool("IsMovingAway", true);
            _bunny.SetRandomEscapeDir();

            // 只播一次哭泣开头，防止重叠
            if (!_bunny.hasPlayedCrySound && _bunny.crySound != null)
            {
                _bunny.enemyAudio.PlayOneShot(_bunny.crySound);
                _bunny.hasPlayedCrySound = true; // 标记为已播放，永远不会再触发
            }
        }

        public void Update()
        {
            // 逃跑移动
            _bunny.agent.speed = _bunny.fleeSpeed;
            _bunny.agent.SetDestination(_bunny.fleeTargetPos);

            // 持续蛊惑周围熊
            _bunny.CryToControlBears();

            // 跑远后回到待机
            float dis = Vector3.Distance(_bunny.transform.position, _bunny.targetPlayer.position);
            if (dis > _bunny.fleeRunDistance)
            {
                _bunny.fsm.SwitchState(_bunny.idleState);
                _bunny.anim.SetBool("ReturnIdle", true);
            }
        }

        public void Exit() {
        }
    }

    // 死亡
    private class BunnyDeathState : IState
    {
        private readonly Zombunny _bunny;
        public BunnyDeathState(Zombunny bunny) => _bunny = bunny;

        public void Enter() { }
        public void Update() { }
        public void Exit() { }
    }
}