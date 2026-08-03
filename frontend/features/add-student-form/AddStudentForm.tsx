import { useRef, useState } from "react"
import { QRCode } from "@/shared/ui/qr-code"
import { CopyField } from "@/shared/ui/copy-field"
import Box from "@mui/material/Box"
import RadioGroup from "@mui/material/RadioGroup"
import FormControlLabel from "@mui/material/FormControlLabel"
import Radio from "@mui/material/Radio"
import RoleGroup from "@/shared/ui/role/RoleGroup"
import CachedIcon from "@mui/icons-material/Cached"
import AuthApi from "@/shared/api/AuthApi"

interface Props {
	isOpen: boolean
}

const AddStudentForm = (props: Props) => {
	const { isOpen } = props

	const [isRefreshing, setIsRefreshing] = useState(false)

	const handleRefreshCode = async () => {
		setIsRefreshing(true)
		// TODO: запросить новый код регистрации
		const newCode = await AuthApi.downloadRegistrationCode()
		if (newCode) {
			setRegisterCode(newCode)
		}
		setIsRefreshing(false)
	}

	const [registerCode, setRegisterCode] = useState<string>("")

	return (
		<Box className="flex flex-col gap-4 ">
			{isOpen && (
				<Box
					className="flex gap-4 p-4 rounded-[20px] w-fit"
					sx={{
						backgroundColor: "white",
					}}
				>
					<QRCode value={registerCode} />
					<Box className="flex flex-col gap-2">
						<Box className="flex items-center gap-2">
							<CopyField value={registerCode} />
							<CachedIcon
								onClick={handleRefreshCode}
								onAnimationEnd={() => setIsRefreshing(false)}
								sx={{
									cursor: "pointer",
									animation: isRefreshing
										? "cached-icon-spin 0.5s ease-in-out infinite"
										: "none",
									"@keyframes cached-icon-spin": {
										from: { transform: "rotate(0deg)" },
										to: { transform: "rotate(360deg)" },
									},
									color: isRefreshing
										? "primary.main"
										: "contrastingSecondary.main",
									"&:hover": {
										color: "primary.main",
									},
								}}
							/>
						</Box>
						<RadioGroup name="role" defaultValue="STUDENT" row>
							<FormControlLabel
								value="STUDENT"
								control={<Radio />}
								label="Студент"
							/>
							<FormControlLabel
								value="GROUP"
								control={<Radio />}
								label="Группа"
							/>
						</RadioGroup>
						<RoleGroup
							roles={[
								{ uuid: "1", name: "Студент", permissions: [] },
							]}
							onAddRole={() => {}}
							onClickRole={() => {}}
						/>
					</Box>
				</Box>
			)}
		</Box>
	)
}
export default AddStudentForm
