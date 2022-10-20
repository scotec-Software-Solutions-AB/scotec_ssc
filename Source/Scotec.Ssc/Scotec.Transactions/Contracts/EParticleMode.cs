namespace Scotec.Transactions
{
    /// <summary>
    ///     The particle mode. If multiple thread uses the same transaction,
    ///     read/write must be synchronized. Multiple threads can access read
    ///     particles at the same time. Only one thread can use a write particle.
    ///     As long as a write particle is activ, other threads can nei´ther use read nor write
    ///     particles. As long as there is atleast one active read particle, no write
    ///     particle can be used.
    ///     Read particles never need to be committed or rolled back.
    /// </summary>
    public enum EParticleMode
    {
        /// <summary>
        ///     Particle write mode.
        /// </summary>
        Write,

        /// <summary>
        ///     Particle read mode.
        /// </summary>
        Read
    }
}