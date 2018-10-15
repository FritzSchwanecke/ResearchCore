namespace IReserachCore.Helper.Functions
{
    public interface IErfc
    {
        /// <summary>
        ///     /* This subprogram computes approximate values for erf(x). */
        ///     /*   (see comments heading CALERF). */
        ///     /*   Author/date: W. J. Cody, January 8, 1985 */
        ///     /* -------------------------------------------------------------------- */
        ///     /*&lt;       INTEGER JINT &gt;*/
        ///     /* S    REAL             X, RESULT */
        ///     /*&lt;       DOUBLE PRECISION X, RESULT &gt;*/
        ///     /* ------------------------------------------------------------------ */
        ///     /*&lt;       JINT = 0 &gt;*/
        ///     /*&lt;       CALL CALERF(X,RESULT,JINT) &gt;*/
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>
        ///     /* S    ERF = RESULT */
        ///     /*&lt;       DERF = RESULT &gt;*/
        ///     /*&lt;       RETURN &gt;*/
        ///     /* ---------- Last card of DERF ---------- */
        ///     /*&lt;       END &gt;*/
        /// </returns>
        double ErfCody(double x);

        /// <summary>
        ///     /* -------------------------------------------------------------------- */
        ///     /* This subprogram computes approximate values for erfc(x). */
        ///     /*   (see comments heading CALERF). */
        ///     /*   Author/date: W. J. Cody, January 8, 1985 */
        ///     /* -------------------------------------------------------------------- */
        ///     /*&lt;       INTEGER JINT &gt;*/
        ///     /* S    REAL             X, RESULT */
        ///     /*&lt;       DOUBLE PRECISION X, RESULT &gt;*/
        ///     /* ------------------------------------------------------------------ */
        ///     /*&lt;       JINT = 1 &gt;*/
        ///     /*&lt;       CALL CALERF(X,RESULT,JINT) &gt;*/
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>
        ///     /* S    ERFC = RESULT */
        ///     /*&lt;       DERFC = RESULT &gt;*/
        ///     /*&lt;       RETURN &gt;*/
        ///     /* ---------- Last card of DERFC ---------- */
        ///     /*&lt;       END &gt;*/
        /// </returns>
        double ErfcCody(double x);

        /// /* This subprogram computes approximate values for exp(x*x) * erfc(x). */
        /// /*   (see comments heading CALERF). */
        /// /*   Author/date: W. J. Cody, March 30, 1987 */
        /// /* ------------------------------------------------------------------ */
        /// /*&lt;       INTEGER JINT &gt;*/
        /// /* S    REAL             X, RESULT */
        /// /*&lt;       DOUBLE PRECISION X, RESULT &gt;*/
        /// /* ------------------------------------------------------------------ */
        /// /*&lt;       JINT = 2 &gt;*/
        /// /*&lt;       CALL CALERF(X,RESULT,JINT) &gt;*/
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns>
        ///     /* S    ERFCX = RESULT */
        ///     /*&lt;       DERFCX = RESULT &gt;*/
        ///     /*&lt;       RETURN &gt;*/
        ///     /* ---------- Last card of DERFCX ---------- */
        ///     /*&lt;       END &gt;*/
        /// </returns>
        double ErfcxCody(double x);
    }
}