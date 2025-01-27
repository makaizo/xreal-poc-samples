using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NRKernal;
using TMPro;

public class DetectFingerNum : MonoBehaviour
{
  private float timer = 0f;
  private float timerInterval = 5f;
  [SerializeField] private TextMeshProUGUI resultText;
  [SerializeField] private TextMeshProUGUI debugText;


  void Start()
  {

  }

  void Update()
  {
    // timerInterval秒ごとに手の状態を確認
    // timer += Time.deltaTime;
    // if (timer >= timerInterval)
    // {
    //   Debug.Log($"{timerInterval}秒経過");
    //   timer = 0f; // タイマーをリセット
    // }
    // else
    // {
    //   return;
    // }

    if (!NRInput.Hands.IsRunning) return;

    CheckFingerPointing(HandEnum.RightHand); // 右手の確認
    // CheckFingerPointing(HandEnum.LeftHand);  // 左手の確認
  }



  void CheckFingerPointing(HandEnum hand)
  {
    HandState handState = NRInput.Hands.GetHandState(hand);
    if (handState == null) return;
    int fingerCount = CountStraightFingers(handState);
    Debug.Log($"[DetectHand]伸びている指の数: {fingerCount}");

    resultText.text = $"Number of Straignt Fingers: {fingerCount}";
  }

  // 伸びている指の数をカウント
  int CountStraightFingers(HandState handState)
  {
    int count = 0;
    if (IsFingerStraight(handState,
        HandJointID.ThumbTip,
        HandJointID.ThumbDistal,
        HandJointID.ThumbProximal,
        HandJointID.ThumbMetacarpal))
    {
      count++;
      Debug.Log("[DetectHand]親指の伸展を検出");
    }
    if (IsFingerStraight(handState,
        HandJointID.IndexTip,
        HandJointID.IndexDistal,
        HandJointID.IndexMiddle,
        HandJointID.IndexProximal))
    {
      count++;
      Debug.Log("[DetectHand]人差し指の伸展を検出");
    }
    if (IsFingerStraight(handState,
        HandJointID.MiddleTip,
        HandJointID.MiddleDistal,
        HandJointID.MiddleMiddle,
        HandJointID.MiddleProximal))
    {
      count++;
      Debug.Log("[DetectHand]中指の伸展を検出");
    }
    if (IsFingerStraight(handState,
        HandJointID.RingTip,
        HandJointID.RingDistal,
        HandJointID.RingMiddle,
        HandJointID.RingProximal))
    {
      count++;
      Debug.Log("[DetectHand]薬指の伸展を検出");
    }
    if (IsFingerStraight(handState,
        HandJointID.PinkyTip,
        HandJointID.PinkyDistal,
        HandJointID.PinkyMiddle,
        HandJointID.PinkyProximal))
    {
      count++;
      Debug.Log("[DetectHand]小指の伸展を検出");
    }
    return count;
  }


  bool IsFingerStraight(HandState handState, HandJointID tip, HandJointID distal, HandJointID middle, HandJointID proximal)
  {
    if (handState == null) return false;

    Vector3 tipPos = handState.GetJointPose(tip).position;
    Vector3 distalPos = handState.GetJointPose(distal).position;
    Vector3 middlePos = handState.GetJointPose(middle).position;
    Vector3 proximalPos = handState.GetJointPose(proximal).position;

    // 関節間のベクトルを正規化
    Vector3 tipToDistal = (distalPos - tipPos).normalized;
    Vector3 distalToMiddle = (middlePos - distalPos).normalized;
    Vector3 middleToProximal = (proximalPos - middlePos).normalized;

    // 関節間の相対角度を計算
    float bendAngle1 = Vector3.Angle(tipToDistal, distalToMiddle);
    float bendAngle2 = Vector3.Angle(distalToMiddle, middleToProximal);

    // 手のサイズに依存しない閾値、親指だけ閾値を変える
    float bendThreshold1 = 40f;
    float bendThreshold2 = 40f;
    if (tip == HandJointID.ThumbTip)
    {
      bendThreshold1 = 35f;
      bendThreshold2 = 35f;
      debugText.text = $"DIP: {bendAngle1:F1}°, PIP: {bendAngle2:F1}°";
      Debug.Log($"[DetectHand]関節の曲がり角度: DIP={bendAngle1:F1}°, PIP={bendAngle2:F1}°");
    }

    // 両方の関節の曲がりが閾値以下なら指が伸びていると判定
    return bendAngle1 < bendThreshold1 && bendAngle2 < bendThreshold2;
  }
}
