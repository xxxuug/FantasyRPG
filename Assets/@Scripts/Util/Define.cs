using UnityEngine;

public class Define
{
    #region Path
    public const string PlayerPath = "@Prefabs/Player";
    public const string SlimeGreenPath = "@Prefabs/Slime Green";
    public const string Gold50Path = "@Prefabs/Gold/Gold 50";
    public const string SlimeCorePath = "@Prefabs/Item/Slime Core";
    public const string BlueShirtPath = "@Prefabs/Equipment/Blue Shirt";
    #endregion

    #region Tag
    public const string EnemyTag = "Enemy";
    #endregion

    #region Animator
    // Player
    public readonly static int isMoveHash = Animator.StringToHash("isMove");
    public readonly static int KeyboardMoveHash = Animator.StringToHash("KeyboardMove");
    public readonly static int JumpHash = Animator.StringToHash("Jump");
    public readonly static int GroundHash = Animator.StringToHash("Ground");
    public readonly static int AttackHash = Animator.StringToHash("Attack");

    // Basic Sword
    public readonly static int isMoveBasicSword = Animator.StringToHash("isMove");
    public readonly static int JumpBasicSword = Animator.StringToHash("Jump");
    public readonly static int GroundBasicSword = Animator.StringToHash("Ground");
    public readonly static int AttackBasicSword = Animator.StringToHash("Attack");

    // Enemy
    public readonly static int HitHash = Animator.StringToHash("Hit");
    public readonly static int DieHash = Animator.StringToHash("Die");
    #endregion
}
