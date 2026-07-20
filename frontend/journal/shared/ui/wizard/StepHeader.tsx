import Box from "@mui/material/Box"
import { ReactNode } from "react"
import { type ClassValue } from "clsx"
import { classNamesTwMerge } from "@/shared/lib/classNamesTwMerge"
import { Label } from "./Label"
import { useWizard } from "./Wizard"
import DoneIcon from "@mui/icons-material/Done"

interface Props {
	children: ReactNode
	stepId?: number | string
	errorMessage?: string | null
	errorIcon?: ReactNode
	completed: boolean
	completedIcon?: ReactNode
	classNames?: {
		label?: ClassValue
		errorMessage?: ClassValue
		number?: ClassValue
	}
}
const STEP_HEADER_CONTAINER_CLASS =
	"cursor-pointer grid grid-cols-[auto_1fr] grid-rows-[auto_1fr] gap-x-2 items-center"
const NUMBER_CLASS =
	"col-start-1 row-start-1 flex items-center justify-center rounded-full border-4"
const LABEL_CLASS = "col-start-2 row-start-1 title title_small"
const ERROR_MESSAGE_CLASS = "col-start-2 row-start-2"

export const StepHeader = (props: Props) => {
	const {
		children,
		stepId,
		errorMessage,
		errorIcon,
		classNames,
		completed,
		completedIcon,
	} = props
	const { currentStep, onStepChange: setCurrentStep } = useWizard()

	return (
		<Box
			className={STEP_HEADER_CONTAINER_CLASS}
			onClick={() => {
				if (stepId !== undefined) {
					setCurrentStep(stepId)
				}
			}}
		>
			{errorIcon ? (
				<Box>{errorIcon}</Box>
			) : (
				<Box
					className={
						classNames?.number
							? classNamesTwMerge(
									NUMBER_CLASS,
									classNames?.number,
								)
							: NUMBER_CLASS
					}
					sx={{
						width: "2.5rem",
						height: "2.5rem",
						backgroundColor:
							stepId === currentStep
								? "primary.main"
								: "secondary.main",
						borderColor: "primary.main",
						color:
							stepId === currentStep
								? "primary.contrastText"
								: "primary.main",
					}}
				>
					{completed
						? (completedIcon ?? (
								<DoneIcon
									sx={{
										filter: `
      drop-shadow(0.6px 0 0 currentColor)
      drop-shadow(-0.6px 0 0 currentColor)
      drop-shadow(0 0.6px 0 currentColor)
      drop-shadow(0 -0.6px 0 currentColor)
    `,
										fontSize: "1.5rem",
									}}
								/>
							))
						: stepId}
				</Box>
			)}
			<Label
				className={
					classNames?.label
						? classNamesTwMerge(LABEL_CLASS, classNames?.label)
						: LABEL_CLASS
				}
			>
				{children}
			</Label>
			<Box
				className={
					classNames?.errorMessage
						? classNamesTwMerge(
								ERROR_MESSAGE_CLASS,
								classNames?.errorMessage,
							)
						: ERROR_MESSAGE_CLASS
				}
				sx={{
					color: "error.main",
					fontSize: "0.8rem",
					visibility: errorMessage ? "visible" : "hidden",
				}}
			>
				{errorMessage ?? "&ZeroWidthSpace"}
			</Box>
		</Box>
	)
}
