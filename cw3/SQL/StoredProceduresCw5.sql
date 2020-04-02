------PROCEDURY DO ZADANIA 5.1

CREATE PROCEDURE checkIfExistsStudies (@studiesName varchar(255))
AS BEGIN
	SELECT 1 FROM Studies WHERE Studies.Name = @studiesName;
END
GO

CREATE PROCEDURE enrollStudent (@studiesName varchar(255), @indexNumber varchar(255))
AS BEGIN

	DECLARE @idEnrollment int =(SELECT TOP 1 Enrollment.IdEnrollment FROM Enrollment 
	INNER JOIN Studies ON Studies.IdStudy = Enrollment.IdStudy
	WHERE Enrollment.Semester = 1 AND Studies.Name = @studiesName ORDER BY Enrollment.StartDate DESC);

	DECLARE @idStudy int = (SELECT Studies.IdStudy FROM Studies WHERE Studies.Name = @studiesName);
	
	IF @idEnrollment IS NULL
	BEGIN
		SET @idEnrollment = (SELECT TOP 1 Enrollment.IdEnrollment FROM Enrollment ORDER BY Enrollment.IdEnrollment DESC)+1;
		INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate)
		VALUES(@idEnrollment, 1, @idStudy, CURRENT_TIMESTAMP);
	END

	UPDATE Student
	SET Student.IdEnrollment = @idEnrollment
	WHERE Student.IndexNumber = @indexNumber

	SELECT * FROM Enrollment WHERE Enrollment.IdEnrollment = @idEnrollment

END

GO
----------PROCEDURY DO ZADANIA 5.2


CREATE PROCEDURE PromoteStudents @Studies varchar(255), @Semester int
AS
BEGIN
	BEGIN TRAN
	
	DECLARE @IdStudies int = (SELECT IdStudy From Studies where Name=@Studies);
	IF @IdStudies IS NULL
		BEGIN
			ROLLBACK;
			SELECT 404;
		END
		
	DECLARE @OldIdEnrollment int = (Select IdEnrollment from Enrollment where IdStudy = @IdStudies AND Semester = @Semester);
	DECLARE @NewIdEnrollment int = (Select IdEnrollment from Enrollment where IdStudy = @IdStudies AND Semester = (@Semester+1));
		
		IF @NewIdEnrollment IS NULL
			BEGIN
				SET @NewIdEnrollment = (Select MAX(IdEnrollment) from Enrollment) + 1;
				INSERT INTO Enrollment(IdEnrollment, Semester, IdStudy, StartDate) VALUES(@NewIdEnrollment, @Semester + 1, @IdStudies, CURRENT_TIMESTAMP);
			END
		UPDATE Student
			SET Student.IdEnrollment = @NewIdEnrollment
			WHERE Student.IdEnrollment = @OldIdEnrollment;
		COMMIT
	SELECT * FROM Enrollment WHERE Enrollment.IdEnrollment = @NewIdEnrollment;
END
