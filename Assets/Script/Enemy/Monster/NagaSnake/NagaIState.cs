public interface NagaIState {
    void Enter();      // เรียกตอนเข้า state นี้
    void Execute();    // เรียกทุก Frame ขณะที่อยู่ใน state
    void Exit();       // เรียกตอนสลับออกจาก state
}
