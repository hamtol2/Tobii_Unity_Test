using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

namespace RonnieJ
{
    public class GazeMove : MonoBehaviour
    {
        public enum State
        {
            Idle, Move
        }

        public State state = State.Idle;

        public Transform marker;
        public float moveSpeed = 3f;
        public float turnSpeed = 540f;

        public float frame = 30f;

        private float fps = 0f;
        private bool hasStarted = false;
        private float startedTime = 0f;
        private Timer timer = new Timer();
        private float distance = 10f;

        void Awake()
        {
            fps = 1f / frame;
        }

        void InitTobii(GazePoint gazePoint)
        {
            hasStarted = true;
            startedTime = gazePoint.Timestamp;

            marker.gameObject.SetActive(true);
        }

        void Update()
        {
            timer.Update(Time.deltaTime);
            if (timer.HasPastSince(fps))
            {
                UpdateMarkerPosition();
            }

            ProcessInput();
            FSM();
        }

        void SetState(State newState)
        {
            state = newState;
        }

        void FSM()
        {
            switch (state)
            {
                case State.Idle: Idle(); break;
                case State.Move: Move(); break;
                default: break;
            }
        }

        void Idle()
        {

        }

        void Move()
        {
            Vector3 dir = marker.position - transform.position;
            dir.y = 0f;

            RotateToward(dir);

            Vector3 targetPos = marker.position;
            targetPos.y = transform.position.y;
            Vector3 framePos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            transform.position = framePos;
        }

        void RotateToward(Vector3 dir)
        {
            if (dir == Vector3.zero)
                return;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            Quaternion frameRot = Quaternion.RotateTowards(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            transform.rotation = frameRot;
        }

        bool HasArrived
        {
            get
            {
                Vector3 startPos = transform.position;
                startPos.y = 0f;
                Vector3 endPos = marker.position;
                endPos.y = 0f;

                return Vector3.Distance(startPos, endPos) <= 0.5f;
            }
        }

        void ProcessInput()
        {
            // Toggle State.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (state == State.Idle) SetState(State.Move);
                else SetState(State.Idle);
            }
        }

        void UpdateMarkerPosition()
        {
            GazePoint gazePoint = TobiiAPI.GetGazePoint();
            if (gazePoint.IsValid)
            {
                if (!hasStarted) InitTobii(gazePoint);
                UpdatePosition(gazePoint);
            }
        }

        void UpdatePosition(GazePoint gazePoint)
        {
            Ray ray = Camera.main.ScreenPointToRay(gazePoint.Screen);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f))
            {
                Vector3 markerPos = hit.point;
                markerPos.y = marker.position.y;
                marker.position = markerPos;
            }
        }
    }
}