import FormControlLabel from "@mui/material/FormControlLabel"
import Radio from "@mui/material/Radio"
import RadioGroup from "@mui/material/RadioGroup"
import { memo, useState } from "react"
import { FormValues } from "./fields"
import FormTextField from "./RegistrationTextField"
import useRegistrationFormStore from "./model/useRegistrationFormStore"
import FormLabel from "@mui/material/FormLabel"
import FormControl from "@mui/material/FormControl"
import FormHelperText from "@mui/material/FormHelperText"

const RegistrationRadioGroup = () => {
	const [role, setRole] = useState<FormValues["personRole"]>("STUDENT")
	const group = useRegistrationFormStore((state) => state.formValues.group)
	const department = useRegistrationFormStore(
		(state) => state.formValues.department,
	)

	const personRole = useRegistrationFormStore(
		(state) => state.formValues.personRole,
	)
	return (
		<>
			<FormControl
				component="fieldset"
				error={personRole.error.length > 0}
			>
				<FormLabel component="legend">Роль</FormLabel>
				<RadioGroup row>
					<FormControlLabel
						value="STUDENT"
						control={<Radio />}
						label="Студент"
						onClick={() => setRole("STUDENT")}
					/>
					<FormControlLabel
						value="TEACHER"
						control={<Radio />}
						label="Преподаватель"
						onClick={() => setRole("TEACHER")}
					/>
				</RadioGroup>
				<FormHelperText>
					{personRole?.error.length > 0 ? personRole.error : " "}
				</FormHelperText>
			</FormControl>

			{role === "STUDENT" && (
				<FormTextField
					label="Группа"
					size="small"
					fieldName="group"
					required
				/>
			)}

			{role === "TEACHER" && (
				<FormTextField
					label="Кафедра"
					size="small"
					fieldName="department"
					required
				/>
			)}
		</>
	)
}

export default memo(RegistrationRadioGroup)
