using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    public float turnSpeed = 10f;  // 旋转速度
    private Vector2 moveInput; //存储wasd移动输入
    private Rigidbody rb;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();//获取刚体
        anim = GetComponent<Animator>();//获取动画
    }

    private void FixedUpdate()
    {
        Move();
        Turning();
        Animating();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    //移动
    void Move()
    {
        // 获取摄像机正前方和右方向量
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;
        //标准化
        camForward.Normalize();
        camRight.Normalize();
        //**以相机为参考，获取移动方向**
        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
        moveDir.Normalize();

        rb.linearVelocity = moveDir * speed;
    }

    //旋转
    void Turning()
    {
        //创建射线(到鼠标位置)
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //射线检测
        RaycastHit floorHit;
        int camRayLength = 100;
        int floormask = 1 << LayerMask.NameToLayer("Floor");
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floormask))
        { 
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0;
            //计算旋转
            Quaternion targetRotation = Quaternion.LookRotation(playerToMouse);
            Quaternion smoothRotation = Quaternion.Lerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothRotation);
        }

    }

    //动画判断
    void Animating()
    {
        bool isW = false;
        if (moveInput.x!=0 || moveInput.y!=0)
        {
            isW = true;
        }
        anim.SetBool("IsWalking", isW);
    }



}
