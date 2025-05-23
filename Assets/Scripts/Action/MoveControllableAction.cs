using UnityEngine;

public class MoveControllableAction : IAction
{
    private Camera attachedCamera;
    private Vector3[] cameraVector;

    private float yaw = 0f;
    private float pitch = 0f;
    private float mouseSensitivity = 2.5f;
    private float maxPitch = 80f;
    private View currentView;
    
    // 인칭 시점 열거체
    public enum View{
        View1 = 0,
        View3 = 1,
        Total = 2
    }

    public MoveControllableAction()
    {
        cameraVector = new Vector3[(int)View.Total];
        cameraVector[(int)View.View1] = new Vector3(0f, 3f, 1f);
        cameraVector[(int)View.View3] = new Vector3(0f, 6f, -5f);
        currentView = View.View1;
    }

    public void Attach(GameContext gameContext, Entity entity, int priority)
    {
        // Controllable Entity는 유일하게 유지
        if(gameContext.controllableEntity != null)
        {
            gameContext.controllableEntity.DetachAction(gameContext, this);
        }
        gameContext.controllableEntity = entity;
    }

    public void Detach(GameContext gameContext, Entity entity)
    {
        if (attachedCamera != null)
        {
            attachedCamera.transform.SetParent(null);
            attachedCamera = null;
        }
        gameContext.controllableEntity = null;
    }

    public bool CanExecute(GameContext gameContext, Entity entity, float deltaTime)
    {
        Rigidbody rigidbody = entity.gameObject.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            Logger.LogWarning($"[ControllableAction] [{entity.gameObject.name}] " +
                $"rigidBody component not found");
            return false;
        }
        return true;
    }

    public void Execute(GameContext gameContext, Entity entity, float deltaTime)
    {
        AttachCamera(gameContext, entity);
        float airJumpCount = entity.GetStat(StatID.AirJumpCount) ?? 0;
        float maxAirJumpCount = entity.GetStat(StatID.MaxAirJumpCount) ?? 0;
        if (airJumpCount != maxAirJumpCount &&
            IsGrounded(gameContext, entity))
        {
            entity.SetStat(StatID.AirJumpCount, maxAirJumpCount);
        }
        ProcessMove(gameContext, entity);
        ProcessJump(gameContext, entity);
        ProcessViewChange(gameContext, entity);
    }

    private void AttachCamera(GameContext gameContext, Entity entity)
    {
        if (attachedCamera != null)
        {
            return;
        }
        attachedCamera = Camera.main;
        if (attachedCamera == null)
        {
            return;
        }
        GameObject gameObject = gameContext.controllableEntity.gameObject;
        attachedCamera.transform.SetParent(gameObject.transform, false);
        adjustCameraTransform(gameContext, entity);
    }

    private void adjustCameraTransform(GameContext gameContext, Entity entity)
    {
        GameObject gameObject = gameContext.controllableEntity.gameObject;
        Vector3 parentScale = gameObject.transform.localScale;
        Vector3 curPoition = cameraVector[(int)currentView];
        attachedCamera.transform.localPosition = new Vector3(
            curPoition.x / (parentScale.x != 0 ? parentScale.x : 1),
            curPoition.y / (parentScale.y != 0 ? parentScale.y : 1),
            curPoition.z / (parentScale.z != 0 ? parentScale.z : 1)
        );
        attachedCamera.transform.localRotation = Quaternion.identity;
    }

    private void ProcessMove(GameContext gameContext, Entity entity)
    {
        GameObject gameObject = entity.gameObject;
        Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        gameObject.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (attachedCamera != null)
        {
            attachedCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 move = gameObject.transform.TransformDirection(inputDir) * 
            (entity.GetStat(StatID.MoveSpeed) ?? 0) * (entity.GetStat(StatID.MoveSpeedScale) ?? 1);
        move.y = rigidBody.velocity.y;
        rigidBody.velocity = move;
    }

    private void ProcessJump(GameContext gameContext, Entity entity)
    {
        bool jump = Input.GetKeyDown(KeyCode.Space);
        if (!jump ||
            !CanJump(entity))
        {
            return;
        }
        GameObject gameObject = entity.gameObject;
        Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
        if (IsGrounded(gameContext, entity))
        {
            rigidBody.AddForce(Vector3.up * (entity.GetStat(StatID.JumpForce) ?? 0),
                ForceMode.Impulse);
            DecreaseStaminaAfterJump(entity);
        }
        else
        {
            // 현재 남은 공중 점프 Count에 따라서 IsGrounded == false 일 때 점프
            float airJumpCount = entity.GetStat(StatID.AirJumpCount) ?? 0;
            if (airJumpCount < 1)
            {
                return;
            }
            airJumpCount--;
            entity.SetStat(StatID.AirJumpCount, airJumpCount);
            rigidBody.AddForce(Vector3.up * (entity.GetStat(StatID.JumpForce) ?? 0),
                ForceMode.Impulse);
            DecreaseStaminaAfterJump(entity);
        }
    }

    private void ProcessViewChange(GameContext gameContext, Entity entity)
    {
        bool viewChange = Input.GetKeyDown(KeyCode.Alpha4);
        if(!viewChange)
        {
            return;
        }
        currentView += 1;
        currentView = (View)((int)currentView % (int)View.Total);
        if(attachedCamera == null)
        {
            return;
        }
        adjustCameraTransform(gameContext, entity);
    }

    private bool CanJump(Entity entity)
    {
        float? stamina = entity.GetStat(StatID.Stamina);
        float? jumpUseStamina = entity.GetStat(StatID.JumpUseStamina);
        return jumpUseStamina == null || 
            (stamina != null && stamina - jumpUseStamina > 0);
    }

    private void DecreaseStaminaAfterJump(Entity entity)
    {
        float? stamina = entity.GetStat(StatID.Stamina);
        float? jumpUseStamina = entity.GetStat(StatID.JumpUseStamina);
        if(jumpUseStamina == null)
        {
            return;
        }
        if(stamina != null)
        {
            entity.SetStat(StatID.Stamina, stamina.Value - jumpUseStamina.Value);
        }
    }

    private bool IsGrounded(GameContext gameContext, Entity entity)
    {
        GameObject gameObject = gameContext.controllableEntity.gameObject;
        if (!gameObject.TryGetComponent<Collider>(out var collider))
        {
            return false;
        }

        Vector3 origin = collider.bounds.center;
        float rayLength = collider.bounds.extents.y + 0.1f;

        return Physics.Raycast(origin, Vector3.down, rayLength);
    }
}
