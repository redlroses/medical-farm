using Logic.Wallet;
using NaughtyAttributes;
using UnityEngine;

namespace Player
{
    public class HeroWallet : MonoBehaviour
    {
        private Wallet _wallet;
        public Wallet Wallet => _wallet;

        private void Awake()
        {
            _wallet = new Wallet();
            _wallet.Account.Changed += (coins => print($"Current amount of coins: {coins}"));
            AddCoins();
        }

        [Button("Add 100 Coins", EButtonEnableMode.Playmode)]
        private void AddCoins()
        {
            _wallet.TryAdd(100);
        }
    }
}