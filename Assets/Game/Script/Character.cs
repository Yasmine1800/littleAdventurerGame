using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    
    private CharacterController _cc;
    public float MoveSpeed = 5f;
    private Vector3 _movementVelocity;
    private PlayerInput _playerInput;

    private float _verticalVelocity;

    public float Gravity = -9.8f;

    private Animator _animator;

    public int Coin;

    //Enemy
    public bool isPlayer = true;
    
    private UnityEngine.AI.NavMeshAgent _navMeshAgent;
    private Transform TargetPlayer;

    //Health
    private Health _health;

    // Damge Caster
    public DamageCaster _damageCaster;

    // Player slides
    private float attackStartTime;

    public float AttackSlideDuration = 0.4f;

    public float AttackSlideSpeed = 0.06f;

    private Vector3 impactOnCharacter;

    public bool IsInvincible;

    public float invincibleDuration = 2f;

    private float AttackAnimationDuration;

    public float SlideSpeed = 9f;

    //State Machine
    public enum CharacterState
    {
        Normal,
        Attacking,
        Dead,
        BeingHit,
        Slide,
        Spawn,

    }

    public CharacterState CurrentState;
    public float SpawnDuration = 2f;
    private float currentSpawnTime;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    private MaterialPropertyBlock _materialPropertyBlock;

    public GameObject ItemToDrop;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();

        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

        if(isPlayer == false)
        {
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = MoveSpeed;
            SwitchStateTo(CharacterState.Spawn);
        } else {
            _playerInput = GetComponent<PlayerInput>();

        }
        Debug.Log("Character Awake");
    }

    private void CalculatePlayerMovement()
    {

        if(_playerInput.MouseButtonDown && _cc.isGrounded){
            SwitchStateTo(CharacterState.Attacking);
            return;
        } else if(_playerInput.SpaceKeyDown && _cc.isGrounded){
            SwitchStateTo(CharacterState.Slide);
            return;
        }

        _movementVelocity.Set(_playerInput.HorizontalInput, 0f, _playerInput.VerticalInput);
        _movementVelocity.Normalize();
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;

        _animator.SetFloat("Speed", _movementVelocity.magnitude);
        _movementVelocity *= MoveSpeed * Time.deltaTime;

        if(_movementVelocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        
        _animator.SetBool("AirBorne", !_cc.isGrounded);
    }

    private void CalculateEnemyMovement()
    {
        if(Vector3.Distance(TargetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(TargetPlayer.position);
            _animator.SetFloat("Speed", 0.2f);
        } else {
            _navMeshAgent.SetDestination(transform.position);
            _animator.SetFloat("Speed", 0f);

            SwitchStateTo(CharacterState.Attacking);
        }
    }



    private void FixedUpdate()
    {

        switch (CurrentState)
        {
            case CharacterState.Normal:
                if(isPlayer)
                    CalculatePlayerMovement();
                else
                    CalculateEnemyMovement();
                break;
            
            case CharacterState.Attacking:

                if(isPlayer){

                    if(Time.time < attackStartTime + AttackSlideDuration){
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / AttackSlideDuration;
                        _movementVelocity = Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);
                    }

                    if(_playerInput.MouseButtonDown && _cc.isGrounded){

                        string currentClipName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        AttackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                        if(currentClipName != "LittleAdventurerAndie_ATTACK_03" && AttackAnimationDuration > 0.5f && AttackAnimationDuration < 0.7f){
                            
                            _playerInput.MouseButtonDown = false;
                            SwitchStateTo(CharacterState.Attacking);
                            CalculatePlayerMovement();
           
                        }
                       
                    }
                }

                break;  
            case CharacterState.Dead:
                return;  

            case CharacterState.BeingHit:

           
                break; 

            case CharacterState.Slide:
                _movementVelocity = transform.forward * SlideSpeed * Time.deltaTime;
                break;

            case CharacterState.Spawn:
                currentSpawnTime -= Time.deltaTime;
                if(currentSpawnTime <= 0){
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
        }

        if(impactOnCharacter.magnitude > 0.2f){
            _movementVelocity = impactOnCharacter * Time.deltaTime;
        }
        impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);

        if(isPlayer){
            if(_cc.isGrounded == false)
                _verticalVelocity = Gravity;
            else
                _verticalVelocity = Gravity * 0.3f;

            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;
            _cc.Move(_movementVelocity);

        } else {
            if(CurrentState != CharacterState.Normal){
                _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;
                _cc.Move(_movementVelocity);
            }
        }

    }

    public void SwitchStateTo(CharacterState newState){

        if(isPlayer){
           
           _playerInput.ClearCache();
        }

        //Existing State
        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if(_damageCaster != null)
                    _damageCaster.DisableDamageCaster();

                if(isPlayer){
                    GetComponent<PlayerVFXManager>().StopBlade();
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Spawn:
                IsInvincible = false;
                break;
        }

        // Entering State
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                _animator.SetTrigger("Attack");

                if(!isPlayer){

                    Quaternion newRotation = Quaternion.LookRotation(TargetPlayer.position - transform.position);
                    transform.rotation = newRotation;
                }

                if(isPlayer)
                {
                    attackStartTime = Time.time;
                }
                break;
            case CharacterState.Dead:
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");

                if(isPlayer){
                    IsInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
            case CharacterState.Slide:
                _animator.SetTrigger("Slide");
                break;
            
            case CharacterState.Spawn:
                IsInvincible = true;
                currentSpawnTime = SpawnDuration;
                StartCoroutine(MaterialAppear());
                break;
        }

        CurrentState = newState;

        Debug.Log("Switch State: " + newState);
    }

    public void SlideAnimationEnds(){
        SwitchStateTo(CharacterState.Normal);
    }

    public void AttackAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void BeingHitAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void ApplyDamage(int damage, Vector3 attackerPos = new Vector3()){

        if(IsInvincible){
            return;
        }
        
        if(_health != null)
            _health.ApplyDamage(damage);
        
        if(!isPlayer){
            GetComponent<EnemyVFXManager>().PlayerBeingHit(attackerPos);
        }

        StartCoroutine(MaterialBlink());

        if(isPlayer){
            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackerPos, 10f);
        } else {
            
            AddImpact(attackerPos, 2.5f);
        }
    }

    IEnumerator DelayCancelInvincible(){
        yield return new WaitForSeconds(invincibleDuration);
        IsInvincible = false;
    }

    private void AddImpact(Vector3 attackPos, float force){
        Vector3 impactDir = transform.position - attackPos;
        impactDir.Normalize();
        impactDir.y = 0;
        impactOnCharacter = impactDir * force;
    } 

    public void EnableDamageCaster() {
        _damageCaster.EnableDamageCaster();
    }

    public void DisableDamageCaster(){
        _damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialBlink(){

        _materialPropertyBlock.SetFloat("_blink", 0.4f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);

        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialDissolve(){
        yield return new WaitForSeconds(2);

        float dissolveTimeDuration = 2f;
        float currentDissovleTime = 0;
        float dissolveHight_start = 20f;
        float dissolveHight_target = -10f;
        float dissolveHight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while(currentDissovleTime < dissolveTimeDuration){

            currentDissovleTime += Time.deltaTime;
            dissolveHight = Mathf.Lerp(dissolveHight_start, dissolveHight_target, currentDissovleTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        } 

        DropItem();  
    }

    public void DropItem(){
        if(ItemToDrop != null){
            Instantiate(ItemToDrop, transform.position, Quaternion.identity);
        }
    }

    public void PickUpItem(PickUp item){
        switch (item.Type)
        {
            case PickUp.PickUpType.Heal:
                AddHealth(item.Value);
                break;
            case PickUp.PickUpType.Coin:
                AddCoin(item.Value);
                break;
        }
    }

    public void AddHealth(int health){
        _health.AddHealth(health);
        GetComponent<PlayerVFXManager>().PlayHealVFX();
    }

    public void AddCoin(int coin){
        Coin += coin;
    }

    public void RotateToTarget(){
        if(CurrentState != CharacterState.Dead){
            transform.LookAt(TargetPlayer, Vector3.up);
        }
    }

    IEnumerator MaterialAppear(){

        float dissolveTimeDuration = SpawnDuration;
        float currentDissovleTime = 0;
        float dissolveHight_start = -10f;
        float dissolveHight_target = 20f;
        float dissolveHight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while(currentDissovleTime < dissolveTimeDuration){

            currentDissovleTime += Time.deltaTime;
            dissolveHight = Mathf.Lerp(dissolveHight_start, dissolveHight_target, currentDissovleTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }

        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
