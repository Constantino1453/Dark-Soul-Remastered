using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class DirectorManager : IActorManagerInterface
{
    public PlayableDirector pd;

    [Header("=== Timeline assets===")]
    public TimelineAsset frontStab;
    public TimelineAsset openBox;
    public TimelineAsset leverUp;

    [Header("=== Assets Settings ===")]
    public ActorManager attacker;
    public ActorManager victim;
    public AudioClip stab;
    public AudioClip openChest;
    public Light DL;

    private void Start()
    {
        pd = GetComponent<PlayableDirector>();
        pd.playOnAwake = false;//�Ԅ�����
        //pd.playableAsset = frontStab;
        //pd.playableAsset = Instantiate(frontStab);//ʵ����һ�ݸ���

    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.D) 
        //    && gameObject.layer == LayerMask.NameToLayer("Enemy")) //ʵΪ����11
        //{
        //    pd.Play();
        //}
    }

    public void playFrontStab(string timelineName, ActorManager attacker, ActorManager victim)
    {
        //if(pd.playableAsset != null) 
        //{
        //    return;
        //}

        if(pd.state == PlayState.Playing)
        {
            return;
        }

        if(timelineName == "frontStab")
        {
            pd.playableAsset = Instantiate(frontStab);//�Դ����ɵ�frontStabΪ Clone��

            //timeline-track-clip-behavier ������
            //ȡ��timeline��
            TimelineAsset timeline = pd.playableAsset as TimelineAsset;// = (TimelineAsset)pd...
            //Debug.Log(timeline.name);
            foreach (var track in timeline.GetOutputTracks())
            {
                //ȡ��track��
                //Debug.Log(track.name);

                if (track.name == "Attacker Script")
                {
                    pd.SetGenericBinding(track, attacker);//����Ա����,����ֱ����track����trackBinding�������衣sourceObject
                    foreach (var clip in track.GetClips())
                    {
                        //ȡ��clip��
                        //Debug.Log(clip.displayName);
                        MySuperPlayableClip myclip = (MySuperPlayableClip)clip.asset;
                        MySuperPlayableBehaviour mybehave = myclip.template;//template��ϵͳ�Լ����ɵ�
                        //
                        //Debug.Log(mybehave.myFloat);
                        mybehave.myFloat = 17372543330.1f;//ע�⣡������prefeb��ֻ��������Դ��(asset)��������������(Hieracrary)
                        //����������Ҫ��TimelinePlayableWizard���ExposedReferences��add������Ȼ�������setReferenceValue������ֵ
                        //pd.SetReferenceValue(myclip.myCamera.exposedName, GameObject.Find("A"));//������Hierarchy���ҵĿ����A������
                        myclip.am.exposedName = System.Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, attacker);
                        //Debug.Log(myclip.am.exposedName);

                    }
                }
                else if (track.name == "Victim Script")
                {
                    pd.SetGenericBinding(track, victim);
                    foreach (var clip in track.GetClips())
                    {
                        MySuperPlayableClip myclip = (MySuperPlayableClip)clip.asset;
                        MySuperPlayableBehaviour mybehave = myclip.template;
                        mybehave.myFloat = 2333.5f;
                        myclip.am.exposedName = GetInstanceID().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, victim);
                        //Debug.Log(myclip.am.exposedName);
                    }
                }
                else if (track.name == "Attacker Animation")
                {
                    pd.SetGenericBinding(track, attacker.ac.animator);
                }
                else if (track.name == "Victim Animation")
                {
                    pd.SetGenericBinding(track, victim.ac.animator);
                }
                else if (track.name == "Stab Audio Track")
                {
                    pd.SetGenericBinding(track, stab);
                }
                else if (track.name == "Light Control Track")
                {
                    pd.SetGenericBinding(track, DL);
                }
            }


            //foreach (var trackBinding in pd.playableAsset.outputs)
            //{
            //    if (trackBinding.streamName == "Attacker Script")
            //    {
            //        pd.SetGenericBinding(trackBinding.sourceObject, attacker);//����Ա����
            //    }
            //    else if (trackBinding.streamName == "Victim Script")
            //    {
            //        pd.SetGenericBinding(trackBinding.sourceObject, victim);
            //    }
            //    else if (trackBinding.streamName == "Attacker Animation")
            //    {
            //        pd.SetGenericBinding(trackBinding.sourceObject, attacker.ac.animator);
            //    }
            //    else if (trackBinding.streamName == "Victim Animation")
            //    {
            //        pd.SetGenericBinding(trackBinding.sourceObject, victim.ac.animator);
            //    }
            //    else if (trackBinding.streamName == "Stab Audio Track")
            //    {
            //        pd.SetGenericBinding(trackBinding.sourceObject, stab);
            //    }
            //    else if (trackBinding.streamName == "Light Control Track")
            //    {
            //        pd.SetGenericBinding(trackBinding.sourceObject, DL);
            //    }
            //}
            pd.Evaluate();//��֤MySuperPlayableClip-CreatePlayable��pd.SetReferenceValue();�ȷ���
            //��������ִ��
            pd.Play();
        }

        else if(timelineName == "openBox")
        {
            //Debug.Log("damn fuck");
            pd.playableAsset = Instantiate(openBox);
            TimelineAsset timeline = pd.playableAsset as TimelineAsset;
            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name == "Player Script")
                {
                    pd.SetGenericBinding(track, attacker);
                    foreach (var clip in track.GetClips())
                    {
                        MySuperPlayableClip myclip = (MySuperPlayableClip)clip.asset;
                        MySuperPlayableBehaviour mybehave = myclip.template;
                        myclip.am.exposedName = System.Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, attacker);

                    }
                }
                else if (track.name == "Box Script")
                {
                    pd.SetGenericBinding(track, victim);
                    foreach (var clip in track.GetClips())
                    {
                        MySuperPlayableClip myclip = (MySuperPlayableClip)clip.asset;
                        MySuperPlayableBehaviour mybehave = myclip.template;
                        myclip.am.exposedName = GetInstanceID().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, victim);
                    }
                }
                else if (track.name == "Player Animation")
                {
                    pd.SetGenericBinding(track, attacker.ac.animator);
                }
                else if (track.name == "Box Animation")
                {
                    pd.SetGenericBinding(track, victim.ac.animator);
                }
                else if (track.name == "Open Audio Track")
                {
                    pd.SetGenericBinding(track, openChest);
                }
            }
            pd.Evaluate();
            pd.Play();
        }

        else if(timelineName == "leverUp")
        {
            //Debug.Log("damn fuck");
            pd.playableAsset = Instantiate(leverUp);
            TimelineAsset timeline = pd.playableAsset as TimelineAsset;
            foreach (var track in timeline.GetOutputTracks())
            {
                if (track.name == "Player Script")
                {
                    pd.SetGenericBinding(track, attacker);
                    foreach (var clip in track.GetClips())
                    {
                        MySuperPlayableClip myclip = (MySuperPlayableClip)clip.asset;
                        MySuperPlayableBehaviour mybehave = myclip.template;
                        myclip.am.exposedName = System.Guid.NewGuid().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, attacker);

                    }
                }
                else if (track.name == "Lever Script")
                {
                    pd.SetGenericBinding(track, victim);
                    foreach (var clip in track.GetClips())
                    {
                        MySuperPlayableClip myclip = (MySuperPlayableClip)clip.asset;
                        MySuperPlayableBehaviour mybehave = myclip.template;
                        myclip.am.exposedName = GetInstanceID().ToString();
                        pd.SetReferenceValue(myclip.am.exposedName, victim);
                    }
                }
                else if (track.name == "Player Animation")
                {
                    pd.SetGenericBinding(track, attacker.ac.animator);
                }
                else if (track.name == "Lever Animation")
                {
                    pd.SetGenericBinding(track, victim.ac.animator);
                }
                else if (track.name == "Audio Track")
                {
                    pd.SetGenericBinding(track, openChest);
                }
            }
            pd.Evaluate();
            pd.Play();
        }
    }
}
