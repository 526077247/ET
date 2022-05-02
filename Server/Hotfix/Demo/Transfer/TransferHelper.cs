namespace ET
{
    public static class TransferHelper
    {
        public static async ETTask Transfer(Unit unit, long sceneInstanceId, string sceneName)
        {
            // 通知客户端开始切场景
            M2C_StartSceneChange m2CStartSceneChange = new M2C_StartSceneChange() {SceneInstanceId = sceneInstanceId, SceneName = sceneName};
            MessageHelper.SendToClient(unit, m2CStartSceneChange);
            
            M2M_UnitTransferRequest request = new M2M_UnitTransferRequest();
            ListComponent<int> Stack = ListComponent<int>.Create();
            request.Unit = unit;
            Entity curEntity = unit;
            Stack.Add(-1);
            while (Stack.Count > 0)
            {
                var index = Stack[Stack.Count - 1];
                if (index != -1)
                {
                    curEntity = request.Entitys[index];
                }
                Stack.RemoveAt(Stack.Count - 1);
                foreach (Entity entity in curEntity.Components.Values)
                {
                    if (entity is ITransfer)
                    {
                        var childIndex = request.Entitys.Count;
                        request.Entitys.Add(entity);
                        Stack.Add(childIndex);
                        request.Map.Add(new RecursiveEntitys
                        {
                            ChildIndex = childIndex,
                            ParentIndex = index,
                            IsChild = 0
                        });
                    }
                }
                foreach (Entity entity in curEntity.Children.Values)
                {
                    if (entity is ITransfer)
                    {
                        var childIndex = request.Entitys.Count;
                        request.Entitys.Add(entity);
                        Stack.Add(childIndex);
                        request.Map.Add(new RecursiveEntitys
                        {
                            ChildIndex = childIndex,
                            ParentIndex = index,
                            IsChild = 1
                        });
                    }
                }
            }
            Stack.Dispose();
            // 删除Mailbox,让发给Unit的ActorLocation消息重发
            unit.RemoveComponent<MailBoxComponent>();
            
            // location加锁
            long oldInstanceId = unit.InstanceId;
            await LocationProxyComponent.Instance.Lock(unit.Id, unit.InstanceId);
            M2M_UnitTransferResponse response = await ActorMessageSenderComponent.Instance.Call(sceneInstanceId, request) as M2M_UnitTransferResponse;
            await LocationProxyComponent.Instance.UnLock(unit.Id, oldInstanceId, response.NewInstanceId);
            unit.Dispose();
        }
    }
}